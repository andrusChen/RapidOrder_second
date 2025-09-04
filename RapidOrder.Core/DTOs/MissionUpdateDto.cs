using RapidOrder.Core.Enums;

namespace RapidOrder.Core.DTOs
{
    public class MissionUpdateDto
    {
        public MissionStatus Status { get; set; }
        public long? AssignedUserId { get; set; }
    }
}
