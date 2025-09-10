using RapidOrder.Core.Enums;

namespace RapidOrder.Core.Entities
{
    public class ActionMap
    {
        public int Id { get; set; }
        public string DeviceCode { get; set; } = null!;
        public int ButtonNumber { get; set; }
        public MissionType MissionType { get; set; }
    }
}
