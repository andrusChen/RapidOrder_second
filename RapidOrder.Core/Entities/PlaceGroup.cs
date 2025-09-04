namespace RapidOrder.Core.Entities
{
    public class PlaceGroup
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public ICollection<Place> Places { get; set; } = new List<Place>();
    }
}
