using TheWeebDenShop.Models;

namespace TheWeebDenShop.Services;

/// <summary>
/// Manages the shopping cart using ASP.NET session state.
/// Extension point: Replace session storage with database persistence for logged-in users.
/// </summary>
public interface ICartService
{
    List<CartItem> GetItems();
    int GetItemCount();
    void AddItem(Product product);
    void UpdateQuantity(string productId, int change);
    void RemoveItem(string productId);
    void Clear();
    decimal GetSubtotal();
    decimal GetTax();
    decimal GetTotal();
}
