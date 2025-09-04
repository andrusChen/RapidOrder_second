using RapidOrder.Core.Enums;

namespace RapidOrder.Core.Entities
{
    public class CallButton
    {
        public int Id { get; set; }
        public string DeviceCode { get; set; } = "";  // RF/Pocsag code you decode
        public string Label { get; set; } = "";

        public MissionType ButtonId { get; set; }
        public int PlaceId { get; set; }
        public Place Place { get; set; } = default!;
        // public ICollection<CallButtonActionMap> ActionMaps { get; set; } = new List<CallButtonActionMap>();
    }
}
