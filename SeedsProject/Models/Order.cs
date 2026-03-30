namespace SeedsProject.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; } // Link to Identity User
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; } // e.g., "Processing", "Shipped"

        // Navigation properties
        public ICollection<OrderItem> OrderItems { get; set; }
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int SeedId { get; set; }

        // Capture the price at the moment of sale
        public decimal PriceAtPurchase { get; set; }
        public int Quantity { get; set; }

        public Order Order { get; set; }
        public Seed Seed { get; set; }
    }

    public class Review
    {
        public int Id { get; set; }
        public int SeedId { get; set; }
        public string UserId { get; set; }
        public int Rating { get; set; } // 1 to 5
        public string Comment { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public Seed Seed { get; set; }
    }
}
