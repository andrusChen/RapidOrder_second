using RapidOrder.Core.Enums;

namespace RapidOrder.Core.Entities
{
    public class User
    {
        public long Id { get; set; }
        public string DisplayName { get; set; } = "";
        public UserRole Role { get; set; }
        public ICollection<Mission> Missions { get; set; } = new List<Mission>();
    }
}
