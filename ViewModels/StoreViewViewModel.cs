using TheWeebDenShop.Models;

namespace TheWeebDenShop.ViewModels;

public class StoreViewViewModel
{
    public Store? Store { get; set; }
    public List<Product> StoreProducts { get; set; } = new();
}
