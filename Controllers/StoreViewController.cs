using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheWeebDenShop.Data;
using TheWeebDenShop.Services;
using TheWeebDenShop.ViewModels;

namespace TheWeebDenShop.Controllers;

public class StoreViewController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly ICartService _cartService;
    private readonly IProductService _productService;

    public StoreViewController(ApplicationDbContext db, ICartService cartService, IProductService productService)
    {
        _db = db;
        _cartService = cartService;
        _productService = productService;
    }

    public async Task<IActionResult> Index(string id)
    {
        var store = await _db.Stores
            .Include(s => s.Products.Where(p => p.IsApproved && !p.IsBanned))
            .FirstOrDefaultAsync(s => s.Id == id);

        var vm = new StoreViewViewModel
        {
            Store = store,
            StoreProducts = store?.Products.OrderByDescending(p => p.CreatedAt).ToList() ?? new()
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddToCart(string productId, string? returnUrl)
    {
        if (!(User.Identity?.IsAuthenticated ?? false))
        {
            var fallbackPath = HttpContext.Request.Path + HttpContext.Request.QueryString;
            return RedirectToAction("Login", "Account", new { returnUrl = returnUrl ?? fallbackPath });
        }

        var product = _productService.GetById(productId);
        if (product != null && product.Stock > 0)
        {
            _cartService.AddItem(product);
            TempData["SuccessMessage"] = $"{product.Name} added to cart!";
        }

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Products");
    }
}
