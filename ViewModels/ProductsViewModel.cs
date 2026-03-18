using TheWeebDenShop.Models;

namespace TheWeebDenShop.ViewModels;

public class ProductsViewModel
{
    public List<Product> FilteredProducts { get; set; } = new();
    public List<string> Genres { get; set; } = new();
    public string? SearchTerm { get; set; }
    public string? SelectedGenre { get; set; }
    public string? SelectedPrice { get; set; }
    public string? CartMessage { get; set; }
}
