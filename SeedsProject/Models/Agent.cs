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

    public class SeedVendorAgent
    {
        // --- Identification ---
        public Guid VendorId { get; private set; }
        public string BusinessName { get; set; }
        public string ContactEmail { get; set; }

        // --- Financials ---
        public decimal TotalRevenue { get; private set; }
        public string Currency { get; set; } = "USD";

        // --- Inventory Management ---
        // Key: Seed Variety Name, Value: Stock Details
        public List<SeedInventoryItem> Inventory { get; set; }

        // --- Rating & Trust ---
        public float ReputationScore { get; set; } // 0.0 to 5.0
        public bool IsVerified { get; set; }

        public SeedVendorAgent(string name, string email)
        {
            VendorId = Guid.NewGuid();
            BusinessName = name;
            ContactEmail = email;
            Inventory = new List<SeedInventoryItem>();
            TotalRevenue = 0m;
        }

        /// <summary>
        /// Processes a sale, updates stock, and adds to revenue.
        /// </summary>
        public bool ProcessSale(string seedSku, int quantity)
        {
            var item = Inventory.FirstOrDefault(i => i.Sku == seedSku);

            if (item != null && item.StockQuantity >= quantity)
            {
                item.StockQuantity -= quantity;
                TotalRevenue += (item.PricePerUnit * quantity);
                return true; // Sale successful
            }

            return false; // Insufficient stock or item not found
        }
    }

    public class SeedInventoryItem
    {
        public string Sku { get; set; }           // Unique Stock Keeping Unit
        public string SeedName { get; set; }      // e.g., "Heirloom Tomato"
        public string Category { get; set; }      // e.g., "Vegetable", "Flower"
        public int StockQuantity { get; set; }    // Number of packets available
        public decimal PricePerUnit { get; set; } // Price per packet
        public DateTime ExpiryDate { get; set; }  // Seeds lose viability over time
        public float GerminationRate { get; set; } // e.g., 0.85 for 85%
    }
}