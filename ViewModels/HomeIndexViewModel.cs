using TheWeebDenShop.Models;

namespace TheWeebDenShop.ViewModels;

public class HomeIndexViewModel
{
    public List<Product> FeaturedProducts { get; set; } = new();
    public string? NewsletterMessage { get; set; }
}
