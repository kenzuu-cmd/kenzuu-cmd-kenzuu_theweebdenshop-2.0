using TheWeebDenShop.Models;

namespace TheWeebDenShop.ViewModels;

public class ProductDetailViewModel
{
    public Product? Product { get; set; }
    public List<Product> RelatedProducts { get; set; } = new();
    public string? CartMessage { get; set; }
}
