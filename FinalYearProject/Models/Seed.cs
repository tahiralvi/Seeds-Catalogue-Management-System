namespace FinalYearProject.Models
{
    public class Seed
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool Approval { get; set; }
        public int Stock { get; set; }
        public string Image { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int AgentID { get; set; }
        public int CategoryID { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        // Navigation properties
        public Agent Agent { get; set; }
        public Category Category { get; set; }
    }
}
