namespace TheWeebDenShop.Models;

/// <summary>
/// Represents a single item in the shopping cart.
/// Stores a snapshot of product data at the time of adding.
/// </summary>
public class CartItem
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Image { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public int Stock { get; set; }

    public decimal Subtotal => Price * Quantity;
}
