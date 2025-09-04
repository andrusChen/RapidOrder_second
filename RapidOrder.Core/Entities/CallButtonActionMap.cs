using RapidOrder.Core.Enums;

namespace RapidOrder.Core.Entities
{
    public class CallButtonActionMap
    {
        public int Id { get; set; }
        public int CallButtonId { get; set; }
        public CallButton CallButton { get; set; } = default!;
        public int ButtonNumber { get; set; }         // e.g. 1..4 on the device
        public MissionType MissionType { get; set; }  // how to interpret that button
    }
}
