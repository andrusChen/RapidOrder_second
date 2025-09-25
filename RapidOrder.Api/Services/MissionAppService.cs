using Microsoft.EntityFrameworkCore;
using RapidOrder.Core.DTOs;
using RapidOrder.Core.Entities;
using RapidOrder.Core.Enums;
using RapidOrder.Infrastructure;

namespace RapidOrder.Api.Services
{
    public class MissionAppService
    {
        private readonly RapidOrderDbContext _db;
        private readonly MissionNotifier _notifier;
        private readonly LearningModeService _learningModeService;

        public MissionAppService(RapidOrderDbContext db, MissionNotifier notifier, LearningModeService learningModeService)
        {
            _db = db;
            _notifier = notifier;
            _learningModeService = learningModeService;
        }

        // Called by the file watcher when it decodes a signal
        public async Task<long> CreateMissionFromSignalAsync(string decoded, int button, DateTime ts, CancellationToken ct = default)
        {
            // 1) Find CallButton by HEX device code
            var callButton = await _db.CallButtons
                .Include(cb => cb.Place)
                .FirstOrDefaultAsync(cb => cb.DeviceCode == decoded, ct);

            if (callButton == null)
            {
                if (_learningModeService.IsLearningModeEnabled)
                {
                    var newCallButton = new CallButton
                    {
                        DeviceCode = decoded,
                        ButtonId = decoded, // Or some other default
                        Label = $"New Button {decoded}",
                        PlaceId = null
                    };
                    _db.CallButtons.Add(newCallButton);
                    await _db.SaveChangesAsync(ct);
                    // Optionally, log that a new button was learned
                    _db.EventLogs.Add(new EventLog
                    {
                        Type = EventType.System,
                        CreatedAt = ts,
                        PayloadJson = $"{\"learnedCallButton\":\"{decoded}\",\"button\":{button}}"
                    });
                    await _db.SaveChangesAsync(ct);
                    return 0; // Don't create a mission for the learning signal
                }

                // Unknown device: log and bail (no Mission)
                _db.EventLogs.Add(new EventLog
                {
                    Type = EventType.MissionCreated, // keeping enum; payload explains it’s unknown
                    CreatedAt = ts,
                    PayloadJson = $"{\"unknownCallButton\":\"{decoded}\",\"button\":{button}}"
                });
                await _db.SaveChangesAsync(ct);
                return 0;
            }

            // 2) Resolve MissionType from ActionMap if defined, else fallback
            var mapped = await _db.ActionMaps.FirstOrDefaultAsync(a => a.DeviceCode == decoded && a.ButtonNumber == button, ct);
            var missionType = mapped != null
                ? mapped.MissionType
                : button switch
                {
                    1 => MissionType.ORDER,
                    2 => MissionType.PAYMENT,
                    3 => MissionType.PAYMENT_EC,
                    4 => MissionType.SERVICE,
                    _ => MissionType.ASSISTANCE // fallback/default
                };

            // 3) Always create a new mission for the mapped Place (status changes are done by staff)
            var mission = new Mission
            {
                PlaceId = callButton.PlaceId,
                Type = missionType,
                Status = MissionStatus.STARTED,
                StartedAt = ts,
                SourceDecoded = decoded,  // HEX device code
                SourceButton = button
            };

            // Auto-assign from place
            if (callButton.Place?.AssignedUserId != null)
            {
                mission.AssignedUserId = callButton.Place.AssignedUserId;
            }

            _db.Missions.Add(mission);

            // EventLog for creation
            _db.EventLogs.Add(new EventLog
            {
                Type = EventType.MissionCreated,
                CreatedAt = ts,
                PlaceId = callButton.PlaceId,
                PayloadJson = $"{\"device\":\"{decoded}\",\"button\":{button}}"
            });

            await _db.SaveChangesAsync(ct);

            // Notify SignalR
            var placeLabel = callButton.Place != null
                ? $"{callButton.Place.Description} (#{callButton.Place.Number})"
                : "Unassigned";

            var dto = new MissionCreatedDto
            {
                Id = mission.Id,
                Type = mission.Type,
                Status = mission.Status,
                StartedAt = mission.StartedAt,
                PlaceId = callButton.PlaceId,
                PlaceLabel = placeLabel,
                SourceDecoded = mission.SourceDecoded,
                SourceButton = mission.SourceButton,
                AssignedUserId = mission.AssignedUserId,
                AssignedUserName = mission.AssignedUser?.DisplayName
            };
            await _notifier.PushCreatedAsync(dto);

            return mission.Id;
        }

        public async Task<Mission?> UpdateMissionAsync(long id, MissionStatus status, long? userId, CancellationToken ct = default)
        {
            var m = await _db.Missions.Include(x => x.Place).FirstOrDefaultAsync(x => x.Id == id, ct);
            if (m == null) return null;

            var now = DateTime.UtcNow;
            m.Status = status;
            if (userId.HasValue) m.AssignedUserId = userId;

            if (status == MissionStatus.ACKNOWLEDGED && m.AcknowledgedAt == null)
                m.AcknowledgedAt = now;

            if (status == MissionStatus.FINISHED && m.FinishedAt == null)
            {
                m.FinishedAt = now;
                if (m.AcknowledgedAt.HasValue)
                    m.MissionDurationSeconds = (long)(m.FinishedAt.Value - m.AcknowledgedAt.Value).TotalSeconds;
                if (m.AcknowledgedAt.HasValue)
                    m.IdleTimeSeconds = (long)(m.AcknowledgedAt.Value - m.StartedAt).TotalSeconds;
            }

            _db.EventLogs.Add(new EventLog { Type = MapEvent(status), CreatedAt = now, MissionId = m.Id, PlaceId = m.PlaceId, UserId = userId });
            await _db.SaveChangesAsync(ct);

            // push update
            var placeLabelForUpdate = m.Place != null
                ? $"{m.Place.Description} (#{m.Place.Number})"
                : "Unassigned";

            var dto = new MissionCreatedDto
            {
                Id = m.Id,
                Type = m.Type,
                Status = m.Status,
                StartedAt = m.StartedAt,
                PlaceId = m.PlaceId,
                PlaceLabel = placeLabelForUpdate,
                SourceDecoded = m.SourceDecoded,
                SourceButton = m.SourceButton,
                AssignedUserId = m.AssignedUserId,
                AssignedUserName = m.AssignedUser?.DisplayName
            };
            await _notifier.PushUpdatedAsync(dto);

            return m;
        }

        private static EventType MapEvent(MissionStatus s) => s switch
        {
            MissionStatus.ACKNOWLEDGED => EventType.MissionAcknowledged,
            MissionStatus.FINISHED => EventType.MissionFinished,
            MissionStatus.CANCELED => EventType.MissionCanceled,
            _ => EventType.MissionCreated
        };
    }
}
