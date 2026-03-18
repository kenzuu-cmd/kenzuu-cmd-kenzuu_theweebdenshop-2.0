using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheWeebDenShop.Services;
using TheWeebDenShop.ViewModels;

namespace TheWeebDenShop.Controllers;

[Authorize]
public class CartController : Controller
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    public IActionResult Index()
    {
        var vm = new CartViewModel
        {
            CartItems = _cartService.GetItems(),
            Subtotal = _cartService.GetSubtotal(),
            Tax = _cartService.GetTax(),
            Total = _cartService.GetTotal()
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
