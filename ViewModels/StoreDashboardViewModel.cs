using System.ComponentModel.DataAnnotations;
using TheWeebDenShop.Models;

namespace TheWeebDenShop.ViewModels;

public class StoreDashboardViewModel
{
    public Store? UserStore { get; set; }
    public List<Product> StoreProducts { get; set; } = new();
    public StoreInputModel StoreInput { get; set; } = new();
    public IFormFile? StoreLogo { get; set; }
}

public class StoreInputModel
{
    [Required(ErrorMessage = "Store name is required.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Store name must be 3–100 characters.")]
    [Display(Name = "Store Name")]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
    [Display(Name = "Store Description")]
    public string? Description { get; set; }
}
