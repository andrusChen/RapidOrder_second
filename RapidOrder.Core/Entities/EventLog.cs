using RapidOrder.Core.Enums;

namespace RapidOrder.Core.Entities
{
    public class EventLog
    {
        public long Id { get; set; }
        public EventType Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public long? MissionId { get; set; }
        public Mission? Mission { get; set; }
        public int? PlaceId { get; set; }
        public Place? Place { get; set; }
        public long? UserId { get; set; }
        public User? User { get; set; }
        public string? PayloadJson { get; set; }
    }
}
