using System.ComponentModel;

namespace DogGo.Models
{
    public class Owner
    {
        public int Id { get; set; }
        public string Email { get; set; }
        [DisplayName("Owner")]
        public string Name { get; set; }
        public string Address { get; set; }
        public int NeighborhoodId { get; set; }
        public Neighborhood Neighborhood { get; set; }
        public string Phone { get; set; }
        public List<Dog> Dogs { get; set; } = new List<Dog>();
    }
}