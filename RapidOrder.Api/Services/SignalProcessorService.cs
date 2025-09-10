using Microsoft.Extensions.Hosting;
using System.Collections.Concurrent;
using System.Diagnostics;
using RapidOrder.Core.Enums;
using RapidOrder.Core.Entities;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;
using System.Collections.Generic;

namespace RapidOrder.Api.Services
{
    public class SignalProcessorService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ConcurrentDictionary<string, DateTime> _lastSeen = new();

        public SignalProcessorService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("📡 Real-time RF signal processor running...");

            var rtlFmStartInfo = new ProcessStartInfo
            {
                FileName = "rtl_fm.exe",
                Arguments = "-f 433.92M -s 22050 -g 20",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var multimonNgStartInfo = new ProcessStartInfo
            {
                FileName = "multimon-ng.exe",
                Arguments = "-a POCSAG1200 -t raw -",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process? rtlFmProcess = null;
            Process? multimonNgProcess = null;

            try
            {
                rtlFmProcess = Process.Start(rtlFmStartInfo);
                multimonNgProcess = Process.Start(multimonNgStartInfo);

                if (rtlFmProcess == null || multimonNgProcess == null)
                {
                    Console.WriteLine("Error starting processes. Ensure rtl_fm.exe and multimon-ng.exe are in the correct path.");
                    return;
                }

                // Asynchronously pipe the output of rtl_fm to the input of multimon-ng
                var pipeTask = Task.Run(async () =>
                {
                    try
                    {
                        await rtlFmProcess.StandardOutput.BaseStream.CopyToAsync(multimonNgProcess.StandardInput.BaseStream, stoppingToken);
                    }
                    catch (OperationCanceledException)
                    {
                        // This is expected when the service is stopping.
                    }
                    finally
                    {
                        multimonNgProcess.StandardInput.Close();
                    }
                }, stoppingToken);

                while (!stoppingToken.IsCancellationRequested)
                {
                    var line = await multimonNgProcess.StandardOutput.ReadLineAsync();

                    if (line == null)
                    {
                        // End of stream, multimon-ng has likely exited.
                        break;
                    }

                    if (!line.Contains("Numeric:")) continue;

                    int idx = line.IndexOf("Numeric:");
                    if (idx < 0) continue;

                    string raw = line.Substring(idx + "Numeric:".Length).Trim();
                    if (string.IsNullOrEmpty(raw)) continue;

                    var (decoded, button) = DecodeRaw(raw);

                    var key = $"{decoded}-{button}";
                    var now = DateTime.UtcNow;
                    if (_lastSeen.TryGetValue(key, out var last) && (now - last).TotalSeconds < 2) continue;
                    _lastSeen[key] = now;

                    await SaveMissionAsync(decoded, button, now, stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Signal processing stopped.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in signal processor: {ex.Message}");
            }
            finally
            {
                if (rtlFmProcess != null && !rtlFmProcess.HasExited)
                {
                    rtlFmProcess.Kill();
                }
                if (multimonNgProcess != null && !multimonNgProcess.HasExited)
                {
                    multimonNgProcess.Kill();
                }
            }
        }

        private static (string decoded, int button) DecodeRaw(string raw)
        {
            var map = new Dictionary<char, char>
            {
                { '.', 'A' }, { 'U', 'B' }, { ' ', 'C' },
                { '-', 'D' }, { ']', 'E' }, { '[', 'F' }
            };

            raw = raw.TrimEnd();
            int button = 0;

            if (raw.Length > 0 && char.IsDigit(raw[^1]))
            {
                button = raw[^1] - '0';
                raw = raw.Substring(0, raw.Length - 1);
            }

            var outChars = raw.Select(ch => map.TryGetValue(ch, out var repl) ? repl : ch).ToArray();
            return (new string(outChars).Trim(), button);
        }

        private async Task SaveMissionAsync(string decoded, int button, DateTime ts, CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var missionAppService = scope.ServiceProvider.GetRequiredService<MissionAppService>();

            await missionAppService.CreateMissionFromSignalAsync(
                decoded,
                button,
                ts,
                ct
            );
        }
    }
}
