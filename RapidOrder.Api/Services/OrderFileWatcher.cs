using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using RapidOrder.Infrastructure;
using RapidOrder.Core.Entities;
using RapidOrder.Core.Enums;
using RapidOrder.Api.Hubs;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;

namespace RapidOrder.Api.Services
{
    // RapidOrder.Api/Services/OrderFileWatcher.cs

    public class OrderFileWatcher : BackgroundService
    {
        private readonly string filePath = "orders_0.txt";
        private long lastPosition = 0;
        private readonly ConcurrentDictionary<string, DateTime> lastSeen = new();
        private readonly IServiceScopeFactory _scopeFactory;

        public OrderFileWatcher(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("📡 RF watcher running...");
            DateTime lastWriteTime = DateTime.MinValue;

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var info = new FileInfo(filePath);
                    if (info.Exists && info.LastWriteTime > lastWriteTime)
                    {
                        lastWriteTime = info.LastWriteTime;
                        await ProcessNewLines(stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Watcher error: " + ex.Message);
                }

                await Task.Delay(300, stoppingToken);
            }
        }

        private async Task ProcessNewLines(CancellationToken ct)
        {
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            fs.Seek(lastPosition, SeekOrigin.Begin);
            using var sr = new StreamReader(fs);

            string? line;
            while ((line = await sr.ReadLineAsync()) != null)
            {
                if (!line.Contains("Numeric:")) continue;

                int idx = line.IndexOf("Numeric:");
                if (idx < 0) continue;

                string raw = line[(idx + "Numeric:".Length)..].Trim();
                if (string.IsNullOrEmpty(raw)) continue;

                var (decoded, button) = DecodeRaw(raw);

                var key = $"{decoded}-{button}";
                var now = DateTime.UtcNow;
                if (lastSeen.TryGetValue(key, out var last) && (now - last).TotalSeconds < 2) continue;
                lastSeen[key] = now;

                // ✅ use async helper
                await SaveMissionAsync(decoded, button, now, ct);
            }

            lastPosition = fs.Position;
        }

        private static (string decoded, int button) DecodeRaw(string raw)
        {
            // same mapping you provided; output is **string** (HEX-like) + button int
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
                raw = raw[..^1];
            }

            var outChars = raw.Select(ch => map.TryGetValue(ch, out var repl) ? repl : ch).ToArray();
            // Keep as string (e.g., "4D3E")
            return (new string(outChars).Trim(), button);
        }


        private async Task SaveMissionAsync(string decoded, int button, DateTime ts, CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var missionAppService = scope.ServiceProvider.GetRequiredService<MissionAppService>();

            var mission = new Mission
            {
                SourceDecoded = decoded,   // ex: "4D3E"
                SourceButton = button,     // ex: 1 (Order), 2 (Payment)
                StartedAt = ts,
                Status = MissionStatus.STARTED
            };

            await missionAppService.CreateMissionFromSignalAsync(
                mission.SourceDecoded,
                (int)mission.SourceButton,
                mission.StartedAt,
                ct
            );
        }

    }
}

