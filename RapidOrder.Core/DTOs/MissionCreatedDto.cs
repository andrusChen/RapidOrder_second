using RapidOrder.Core.Enums;

namespace RapidOrder.Core.DTOs
{
    public class MissionCreatedDto
    {
        public long Id { get; set; }
        public MissionType Type { get; set; }
        public MissionStatus Status { get; set; }
        public DateTime StartedAt { get; set; }
        public int? PlaceId { get; set; }
        public string PlaceLabel { get; set; } = "";
        public string? SourceDecoded { get; set; }
        public int? SourceButton { get; set; }
        public long? AssignedUserId { get; set; }
        public string? AssignedUserName { get; set; }
    }
}
