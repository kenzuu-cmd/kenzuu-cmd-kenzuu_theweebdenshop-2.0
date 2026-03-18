using TheWeebDenShop.Models;

namespace TheWeebDenShop.ViewModels;

public class CheckoutViewModel
{
    public CheckoutFormModel CheckoutForm { get; set; } = new();
    public List<CartItem> CartItems { get; set; } = new();
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }
    public string? SuccessMessage { get; set; }
}
