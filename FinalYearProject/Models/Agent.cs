namespace FinalYearProject.Models
{
    public class Agent
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime CreatedDate { get; set; }

        // Navigation property
        public ICollection<Seed> Seeds { get; set; }
    }
}
