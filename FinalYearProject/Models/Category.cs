namespace FinalYearProject.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }

        // Navigation property
        public ICollection<Seed> Seeds { get; set; }
    }
}
