using TheWeebDenShop.Models;

namespace TheWeebDenShop.ViewModels;

public class CartViewModel
{
    public List<CartItem> CartItems { get; set; } = new();
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }
}
