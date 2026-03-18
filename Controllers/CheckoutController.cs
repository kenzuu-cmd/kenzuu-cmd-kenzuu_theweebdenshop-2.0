using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheWeebDenShop.Models;
using TheWeebDenShop.Services;
using TheWeebDenShop.ViewModels;

namespace TheWeebDenShop.Controllers;

[Authorize]
public class CheckoutController : Controller
{
    private readonly ICartService _cartService;

    public CheckoutController(ICartService cartService)
    {
        _cartService = cartService;
    }

    public IActionResult Index()
    {
        var items = _cartService.GetItems();
        if (items.Count == 0)
        {
            return RedirectToAction("Index", "Cart");
        }

        var vm = new CheckoutViewModel
        {
            CartItems = items,
            Subtotal = _cartService.GetSubtotal(),
            Tax = _cartService.GetTax(),
            Total = _cartService.GetTotal(),
            SuccessMessage = TempData["SuccessMessage"] as string
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Index(CheckoutViewModel vm)
    {
        vm.CartItems = _cartService.GetItems();
        vm.Subtotal = _cartService.GetSubtotal();
        vm.Tax = _cartService.GetTax();
        vm.Total = _cartService.GetTotal();

        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        _cartService.Clear();
        TempData["SuccessMessage"] = "Thank you for your order! Your manga is on its way! 🎉";
        return RedirectToAction("Index", "Home");
    }
}
