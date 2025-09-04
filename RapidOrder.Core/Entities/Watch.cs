namespace RapidOrder.Core.Entities
{
    public class Watch
    {
        public int Id { get; set; }
        public string Serial { get; set; } = "";
        public long? AssignedUserId { get; set; }
        public User? AssignedUser { get; set; }
        public int BatteryPercent { get; set; }
        public DateTime? LastSeenAt { get; set; }
    }
}
