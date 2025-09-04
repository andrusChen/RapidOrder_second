namespace RapidOrder.Core.Entities
{
    public class Place
    {
        public int Id { get; set; }
        public int Number { get; set; }               // table number
        public string Description { get; set; } = "";
        public int? PlaceGroupId { get; set; }
        public PlaceGroup? PlaceGroup { get; set; }
        public ICollection<Mission> Missions { get; set; } = new List<Mission>();
        public ICollection<CallButton> CallButtons { get; set; } = new List<CallButton>();
    }
}
