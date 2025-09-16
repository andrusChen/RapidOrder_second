namespace RapidOrder.Core.Enums
{
    public enum EventType
    {
        MissionCreated = 0,
        MissionAcknowledged = 1,
        MissionFinished = 2,
        MissionCanceled = 3,
        System = 4,
        WatchHeartbeat = 10,
        BatteryUpdate = 11
    }
}
