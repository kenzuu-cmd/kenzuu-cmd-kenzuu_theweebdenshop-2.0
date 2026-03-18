using Microsoft.AspNetCore.Mvc;
using TheWeebDenShop.Models;
using TheWeebDenShop.Services;
using TheWeebDenShop.ViewModels;

namespace TheWeebDenShop.Controllers;

public class HomeController : Controller
{
    private readonly IProductService _productService;
    private readonly INewsletterService _newsletterService;

    public HomeController(IProductService productService, INewsletterService newsletterService)
    {
        _productService = productService;
        _newsletterService = newsletterService;
    }

    public IActionResult Index()
    {
        var vm = new HomeIndexViewModel
        {
            FeaturedProducts = _productService.GetFeatured(8),
            NewsletterMessage = TempData["NewsletterMessage"] as string
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Newsletter(string email)
    {
        if (!string.IsNullOrWhiteSpace(email))
        {
            await _newsletterService.SubscribeAsync(email);
            TempData["NewsletterMessage"] = "Thank you for subscribing! Check your email for exclusive deals.";
        }
        return RedirectToAction("Index");
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult Contact()
    {
        var vm = new ContactViewModel
        {
            SuccessMessage = TempData["SuccessMessage"] as string
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Contact(ContactViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        TempData["SuccessMessage"] = "Thank you for your message! We will get back to you soon.";
        return RedirectToAction("Contact");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Terms()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        ViewData["RequestId"] = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        return View();
    }
}
