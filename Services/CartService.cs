using System.Text.Json;
using TheWeebDenShop.Models;

namespace TheWeebDenShop.Services;

/// <summary>
/// Session-backed cart service. Stores cart as JSON in HttpContext.Session.
/// Tax rate matches the original site (8%).
/// </summary>
public class CartService : ICartService
{
    private const string CartSessionKey = "ShoppingCart";
    private const decimal TaxRate = 0.08m;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CartService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ISession Session => _httpContextAccessor.HttpContext!.Session;

    private List<CartItem> LoadCart()
    {
        var json = Session.GetString(CartSessionKey);
        if (string.IsNullOrEmpty(json)) return new List<CartItem>();
        return JsonSerializer.Deserialize<List<CartItem>>(json) ?? new List<CartItem>();
    }

    private void SaveCart(List<CartItem> items)
    {
        var json = JsonSerializer.Serialize(items);
        Session.SetString(CartSessionKey, json);
    }

    public List<CartItem> GetItems() => LoadCart();

    public int GetItemCount() => LoadCart().Sum(i => i.Quantity);

    public void AddItem(Product product)
    {
        var items = LoadCart();
        var existing = items.FirstOrDefault(i => i.Id == product.Id);

        if (existing != null)
        {
            if (existing.Quantity < product.Stock)
            {
                existing.Quantity++;
            }
        }
        else if (product.Stock > 0)
        {
            items.Add(new CartItem
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Image = product.Image,
                Quantity = 1,
                Stock = product.Stock
            });
        }

        SaveCart(items);
    }

    public void UpdateQuantity(string productId, int change)
    {
        var items = LoadCart();
        var item = items.FirstOrDefault(i => i.Id == productId);
        if (item == null) return;

        var newQty = item.Quantity + change;
        if (newQty <= 0)
        {
            items.Remove(item);
        }
        else if (newQty <= item.Stock)
        {
            item.Quantity = newQty;
        }

        SaveCart(items);
    }

    public void RemoveItem(string productId)
    {
        var items = LoadCart();
        items.RemoveAll(i => i.Id == productId);
        SaveCart(items);
    }

    public void Clear()
    {
        SaveCart(new List<CartItem>());
    }

    public decimal GetSubtotal() => LoadCart().Sum(i => i.Subtotal);

    public decimal GetTax() => GetSubtotal() * TaxRate;

    public decimal GetTotal() => GetSubtotal() + GetTax();
}
