using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeedsProject.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    public class SeedCategory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(150)]
        public string Slug { get; set; }

        public string Description { get; set; }

        // --- Hierarchy Properties ---

        public int? ParentCategoryId { get; set; }

        [ForeignKey("ParentCategoryId")]
        public virtual SeedCategory ParentCategory { get; set; }

        public virtual ICollection<SeedCategory> SubCategories { get; set; } = new List<SeedCategory>();

        // --- Botanical & Growth Metadata ---

        public string ScientificFamily { get; set; } // e.g., Brassicaceae

        public LifeCycleType LifeCycle { get; set; } // Annual, Perennial, etc.

        public string IdealSowingMonths { get; set; } // Stored as CSV or JSON (e.g., "3,4,5,9")

        public int? MinHardinessZone { get; set; }

        public int? MaxHardinessZone { get; set; }

        // --- E-commerce & UI Properties ---

        public string IconUrl { get; set; }

        public string BannerImageUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsFeatured { get; set; } = false;

        public int DisplayOrder { get; set; } = 0;

        // Link to educational content/growing guides
        public string GuideUrl { get; set; }

        // SEO Metadata
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }

        // --- System Properties ---

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Denormalized count for performance (Optional: updated via background job)
        public int ProductCount { get; set; }
    }

    public enum LifeCycleType
    {
        Annual = 1,
        Biennial = 2,
        Perennial = 3
    }
}