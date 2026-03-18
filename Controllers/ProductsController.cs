using Microsoft.AspNetCore.Mvc;
using TheWeebDenShop.Services;
using TheWeebDenShop.ViewModels;

namespace TheWeebDenShop.Controllers;

public class ProductsController : Controller
{
    private readonly IProductService _productService;
    private readonly ICartService _cartService;

    public ProductsController(IProductService productService, ICartService cartService)
    {
        _productService = productService;
        _cartService = cartService;
    }

    public IActionResult Index(string? search, string? genre, string? price)
    {
        decimal? minPrice = null, maxPrice = null;
        if (!string.IsNullOrEmpty(price))
        {
            var parts = price.Split('-');
            if (parts.Length == 2
                && decimal.TryParse(parts[0], out var min)
                && decimal.TryParse(parts[1], out var max))
            {
                minPrice = min;
                maxPrice = max;
            }
        }

        var vm = new ProductsViewModel
        {
            FilteredProducts = _productService.Search(search, genre, minPrice, maxPrice),
            Genres = _productService.GetGenres(),
            SearchTerm = search,
            SelectedGenre = genre,
            SelectedPrice = price,
            CartMessage = TempData["CartMessage"] as string
        };
        return View(vm);
    }

    public IActionResult Detail(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return RedirectToAction("Index");
        }

        var product = _productService.GetById(id);
        var vm = new ProductDetailViewModel
        {
            Product = product,
            CartMessage = TempData["CartMessage"] as string
        };

        if (product != null)
        {
            vm.RelatedProducts = _productService.GetByGenre(product.Genre)
                .Where(p => p.Id != product.Id)
                .OrderBy(_ => Random.Shared.Next())
                .Take(4)
                .ToList();
        }

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddToCart(string productId, string? returnUrl)
    {
        if (!(User.Identity?.IsAuthenticated ?? false))
        {
            TempData["Message"] = "Please login or register to add items to your cart.";
            return RedirectToAction("Login", "Account", new { returnUrl = returnUrl ?? Url.Action("Index") });
        }

        var product = _productService.GetById(productId);
        if (product != null && product.Stock > 0)
        {
            _cartService.AddItem(product);
            TempData["CartMessage"] = $"{product.Name} added to cart!";
        }

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        return RedirectToAction("Index");
    }
}
