namespace SeedsProject.Models
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


    public class SeedInventoryItem
    {
        public string SeedName { get; set; }      // e.g., "Heirloom Tomato"
        public string Category { get; set; }      // e.g., "Vegetable", "Flower"
        public int StockQuantity { get; set; }    // Number of packets available
        public decimal PricePerUnit { get; set; } // Price per packet
        public DateTime ExpiryDate { get; set; }  // Seeds lose viability over time
        public float GerminationRate { get; set; } // e.g., 0.85 for 85%
    }
}