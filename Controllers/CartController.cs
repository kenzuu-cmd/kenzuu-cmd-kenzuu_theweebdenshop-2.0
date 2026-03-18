using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheWeebDenShop.Data;
using TheWeebDenShop.Services;
using TheWeebDenShop.ViewModels;

namespace TheWeebDenShop.Controllers;

[Authorize]
public class CartController : Controller
{
    private readonly ICartService _cartService;
    private readonly ApplicationDbContext _dbContext;

    public CartController(ICartService cartService, ApplicationDbContext dbContext)
    {
        _cartService = cartService;
        _dbContext = dbContext;
    }

    public IActionResult Index()
    {
        var cartItems = _cartService.GetItems();
        var productIds = cartItems.Select(i => i.Id).Distinct().ToList();
        var existingProductIds = _dbContext.Products
            .Where(p => productIds.Contains(p.Id))
            .Select(p => p.Id)
            .ToHashSet();

        var unavailableProductIds = cartItems
            .Where(i => !existingProductIds.Contains(i.Id))
            .Select(i => i.Id)
            .ToHashSet();

        var availableSubtotal = cartItems
            .Where(i => existingProductIds.Contains(i.Id))
            .Sum(i => i.Subtotal);

        var tax = availableSubtotal * 0.08m;

        var vm = new CartViewModel
        {
            CartItems = cartItems,
            UnavailableProductIds = unavailableProductIds,
            Subtotal = availableSubtotal,
            Tax = tax,
            Total = availableSubtotal + tax
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateQuantity(string productId, int change)
    {
        _cartService.UpdateQuantity(productId, change);
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RemoveItem(string productId)
    {
        _cartService.RemoveItem(productId);
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ClearCart()
    {
        _cartService.Clear();
        return RedirectToAction("Index");
    }
}
