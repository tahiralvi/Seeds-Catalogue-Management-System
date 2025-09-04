using System.ComponentModel.DataAnnotations;

namespace FinalYearProject.Models;

public class SeedCreateViewModel
{
    [Required(ErrorMessage = "Seed name is required")]
    [StringLength(255, ErrorMessage = "Name cannot exceed 255 characters")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Description is required")]
    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string Description { get; set; }

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, 10000, ErrorMessage = "Price must be between 0.01 and 10,000")]
    public decimal Price { get; set; }

    public bool Approval { get; set; }

    [Required(ErrorMessage = "Stock quantity is required")]
    [Range(0, 100000, ErrorMessage = "Stock must be between 0 and 100,000")]
    public int Stock { get; set; }

    [Url(ErrorMessage = "Please enter a valid URL")]
    public string Image { get; set; }

    [Required(ErrorMessage = "Expiry date is required")]
    [DataType(DataType.Date)]
    [FutureDate(ErrorMessage = "Expiry date must be in the future")]
    public DateTime ExpiryDate { get; set; }

    [Required(ErrorMessage = "Agent ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a valid agent")]
    public int AgentID { get; set; }

    [Required(ErrorMessage = "Category ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a valid category")]
    public int CategoryID { get; set; }
}

// Custom validation attribute for future date
public class FutureDateAttribute : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        if (value is DateTime date)
        {
            return date > DateTime.Today;
        }
        return false;
    }
}