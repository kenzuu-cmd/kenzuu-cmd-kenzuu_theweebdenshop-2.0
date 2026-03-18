using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheWeebDenShop.Data;
using TheWeebDenShop.Models;
using TheWeebDenShop.Services;
using TheWeebDenShop.ViewModels;

namespace TheWeebDenShop.Controllers;

[Authorize]
public class StoreController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<User> _userManager;
    private readonly IImageUploadService _imageService;
    private readonly ILogger<StoreController> _logger;

    public StoreController(ApplicationDbContext db,
                           UserManager<User> userManager,
                           IImageUploadService imageService,
                           ILogger<StoreController> logger)
    {
        _db = db;
        _userManager = userManager;
        _imageService = imageService;
        _logger = logger;
    }

    public async Task<IActionResult> Dashboard()
    {
        var userId = _userManager.GetUserId(User);
        if (!int.TryParse(userId, out var ownerId))
        {
            return Challenge();
        }

        // Read fresh data every request so deleted/recreated store names show immediately.
        var store = await _db.Stores
            .AsNoTracking()
            .Include(s => s.Products)
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync(s => s.OwnerId == ownerId);

        var vm = new StoreDashboardViewModel
        {
            UserStore = store,
            StoreProducts = store?.Products.OrderByDescending(p => p.CreatedAt).ToList() ?? new()
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateStore(StoreDashboardViewModel vm)
    {
        var userId = _userManager.GetUserId(User);
        if (!int.TryParse(userId, out var ownerId))
        {
            return Challenge();
        }

        // AsNoTracking avoids any stale tracked entity checks during create.
        var existing = await _db.Stores.AsNoTracking().AnyAsync(s => s.OwnerId == ownerId);
        if (existing)
        {
            TempData["ErrorMessage"] = "You already have a store.";
            return RedirectToAction("Dashboard");
        }

        if (!ModelState.IsValid)
        {
            vm.UserStore = null;
            vm.StoreProducts = new();
            return View("Dashboard", vm);
        }

        string? logoPath = null;
        if (vm.StoreLogo != null)
        {
            logoPath = await _imageService.SaveImageAsync(vm.StoreLogo, "stores");
            if (logoPath == null)
            {
                ModelState.AddModelError("StoreLogo", "Invalid image. Use JPG, PNG, WebP, or GIF under 5 MB.");
                vm.UserStore = null;
                vm.StoreProducts = new();
                return View("Dashboard", vm);
            }
        }

        var store = new Store
        {
            OwnerId = ownerId,
            Name = vm.StoreInput.Name,
            Description = vm.StoreInput.Description ?? string.Empty,
            LogoPath = logoPath
        };

        _db.Stores.Add(store);
        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = "Your store has been created! Start adding manga.";
        return RedirectToAction("Dashboard");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteManga(string mangaId)
    {
        var userId = _userManager.GetUserId(User);
        if (!int.TryParse(userId, out var ownerId))
        {
            return Challenge();
        }
        var store = await _db.Stores.FirstOrDefaultAsync(s => s.OwnerId == ownerId);
        if (store == null) return RedirectToAction("Dashboard");

        var manga = await _db.Products.FirstOrDefaultAsync(p => p.Id == mangaId && p.StoreId == store.Id);
        if (manga == null)
        {
            TempData["ErrorMessage"] = "Manga not found or access denied.";
            return RedirectToAction("Dashboard");
        }

        _imageService.DeleteImage(manga.Image);
        _db.Products.Remove(manga);
        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = $"'{manga.Name}' has been deleted.";
        return RedirectToAction("Dashboard");
    }

    /// <summary>
    /// Deletes the current user's store and all store-owned data.
    /// Teacher note: Main logic for "Delete Store" is here.
    /// Debug tip: If delete fails or old name still appears, check logs for ownerId,
    /// ensure all owner stores were removed, and confirm Dashboard uses AsNoTracking.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteStore()
    {
        var userId = _userManager.GetUserId(User);
        if (!int.TryParse(userId, out var ownerId))
        {
            return Challenge();
        }

        var stores = await _db.Stores
            .Include(s => s.Products)
            .Where(s => s.OwnerId == ownerId)
            .ToListAsync();

        if (stores.Count == 0)
        {
            TempData["ErrorMessage"] = "No store found to delete.";
            return RedirectToAction("Dashboard");
        }

        await using var transaction = await _db.Database.BeginTransactionAsync();

        try
        {
            // Safety cleanup: remove every store row for this owner so old profile names cannot linger.
            var allListings = stores.SelectMany(s => s.Products).ToList();

            foreach (var listing in allListings)
            {
                _imageService.DeleteImage(listing.Image);
            }

            foreach (var ownerStore in stores)
            {
                _imageService.DeleteImage(ownerStore.LogoPath);
                _imageService.DeleteImage(ownerStore.BannerPath);
            }

            _db.Products.RemoveRange(allListings);
            _db.Stores.RemoveRange(stores);
            await _db.SaveChangesAsync();

            // Defensive cleanup: clear possible stale store/profile references after delete.
            HttpContext.Session.Remove("CurrentStoreId");
            HttpContext.Session.Remove("CurrentStoreName");
            HttpContext.Session.Remove("StoreProfileName");
            _db.ChangeTracker.Clear();

            await transaction.CommitAsync();

            TempData["SuccessMessage"] = "Your store and listings were deleted. You can create a new store anytime.";
            return RedirectToAction("Dashboard");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Failed to delete store for owner {OwnerId}", ownerId);
            TempData["ErrorMessage"] = "Delete failed. Please try again. If it keeps failing, contact support.";
            return RedirectToAction("Dashboard");
        }
    }

    // ── Add Manga ──

    [HttpGet]
    public async Task<IActionResult> AddManga()
    {
        var userId = _userManager.GetUserId(User);
        if (!int.TryParse(userId, out var ownerId))
        {
            return Challenge();
        }
        var hasStore = await _db.Stores.AnyAsync(s => s.OwnerId == ownerId);
        if (!hasStore)
        {
            TempData["ErrorMessage"] = "Please create a store first.";
            return RedirectToAction("Dashboard");
        }
        return View(new MangaFormModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddManga(MangaFormModel input)
    {
        var userId = _userManager.GetUserId(User);
        if (!int.TryParse(userId, out var ownerId))
        {
            return Challenge();
        }
        var store = await _db.Stores.FirstOrDefaultAsync(s => s.OwnerId == ownerId);
        if (store == null)
        {
            TempData["ErrorMessage"] = "Please create a store first.";
            return RedirectToAction("Dashboard");
        }

        if (input.CoverImage == null || input.CoverImage.Length == 0)
        {
            ModelState.AddModelError("CoverImage", "A cover image is required for new listings.");
        }

        if (!ModelState.IsValid)
            return View(input);

        var imagePath = await _imageService.SaveImageAsync(input.CoverImage!, "manga");
        if (imagePath == null)
        {
            ModelState.AddModelError("CoverImage", "Invalid image. Use JPG, PNG, WebP, or GIF under 5 MB.");
            return View(input);
        }

        var product = new Product
        {
            Id = Guid.NewGuid().ToString(),
            Name = input.Title,
            Author = input.Author,
            Description = input.Description ?? string.Empty,
            Short = input.Description?.Length > 100
                ? input.Description[..100] + "..."
                : input.Description ?? string.Empty,
            Genre = input.Genre,
            Price = input.Price,
            Stock = input.Stock,
            Volumes = input.Volumes,
            Image = imagePath,
            StoreId = store.Id,
            IsApproved = true,
            Rating = 0
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = $"'{product.Name}' has been added to your store!";
        return RedirectToAction("Dashboard");
    }

    // ── Edit Manga ──

    [HttpGet]
    public async Task<IActionResult> EditManga(string id)
    {
        var userId = _userManager.GetUserId(User);
        if (!int.TryParse(userId, out var ownerId))
        {
            return Challenge();
        }
        var store = await _db.Stores.FirstOrDefaultAsync(s => s.OwnerId == ownerId);
        if (store == null) return RedirectToAction("Dashboard");

        var manga = await _db.Products.FirstOrDefaultAsync(p => p.Id == id && p.StoreId == store.Id);
        if (manga == null)
        {
            TempData["ErrorMessage"] = "Manga not found or access denied.";
            return RedirectToAction("Dashboard");
        }

        var input = new MangaFormModel
        {
            Id = manga.Id,
            Title = manga.Name,
            Author = manga.Author,
            Description = manga.Description,
            Genre = manga.Genre,
            Price = manga.Price,
            Stock = manga.Stock,
            Volumes = manga.Volumes,
            ExistingImagePath = manga.Image
        };

        return View(input);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditManga(MangaFormModel input)
    {
        var userId = _userManager.GetUserId(User);
        if (!int.TryParse(userId, out var ownerId))
        {
            return Challenge();
        }
        var store = await _db.Stores.FirstOrDefaultAsync(s => s.OwnerId == ownerId);
        if (store == null) return RedirectToAction("Dashboard");

        var manga = await _db.Products.FirstOrDefaultAsync(p => p.Id == input.Id && p.StoreId == store.Id);
        if (manga == null)
        {
            TempData["ErrorMessage"] = "Manga not found or access denied.";
            return RedirectToAction("Dashboard");
        }

        if (!ModelState.IsValid)
            return View(input);

        manga.Name = input.Title;
        manga.Author = input.Author;
        manga.Description = input.Description ?? string.Empty;
        manga.Short = (input.Description?.Length > 100
            ? input.Description[..100] + "..."
            : input.Description) ?? string.Empty;
        manga.Genre = input.Genre;
        manga.Price = input.Price;
        manga.Stock = input.Stock;
        manga.Volumes = input.Volumes;

        if (input.CoverImage != null && input.CoverImage.Length > 0)
        {
            var newPath = await _imageService.SaveImageAsync(input.CoverImage, "manga");
            if (newPath == null)
            {
                ModelState.AddModelError("CoverImage", "Invalid image. Use JPG, PNG, WebP, or GIF under 5 MB.");
                return View(input);
            }
            _imageService.DeleteImage(manga.Image);
            manga.Image = newPath;
        }

        await _db.SaveChangesAsync();
        TempData["SuccessMessage"] = $"'{manga.Name}' has been updated.";
        return RedirectToAction("Dashboard");
    }
}
