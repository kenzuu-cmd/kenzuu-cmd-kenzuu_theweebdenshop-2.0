using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheWeebDenShop.Data;
using TheWeebDenShop.Models;
using TheWeebDenShop.ViewModels;

namespace TheWeebDenShop.Controllers;

[Authorize(Policy = "RequireAdminRole")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<User> _userManager;

    public AdminController(ApplicationDbContext db, UserManager<User> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> Dashboard()
    {
        var users = await _userManager.Users.ToListAsync();
        var userRoles = new Dictionary<int, List<string>>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userRoles[user.Id] = roles.ToList();
        }

        var stores = await _db.Stores.ToListAsync();
        var userStores = stores.ToDictionary(s => s.OwnerId);

        var userListings = await _db.Products
            .Where(p => p.StoreId != null)
            .Include(p => p.Store)
            .ToListAsync();

        var storeListingCounts = userListings
            .Where(p => p.StoreId != null)
            .GroupBy(p => p.StoreId!)
            .ToDictionary(g => g.Key, g => g.Count());

        var vm = new AdminDashboardViewModel
        {
            TotalUsers = users.Count,
            TotalStores = stores.Count,
            TotalListings = userListings.Count,
            BannedListings = userListings.Count(p => p.IsBanned),
            Users = users,
            UserRoles = userRoles,
            UserStores = userStores,
            StoreListingCounts = storeListingCounts,
            AllUserListings = userListings.OrderByDescending(p => p.CreatedAt).ToList()
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BanListing(string listingId)
    {
        var listing = await _db.Products.FindAsync(listingId);
        if (listing != null)
        {
            listing.IsBanned = true;
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = $"'{listing.Name}' has been banned.";
        }
        return RedirectToAction("Dashboard");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UnbanListing(string listingId)
    {
        var listing = await _db.Products.FindAsync(listingId);
        if (listing != null)
        {
            listing.IsBanned = false;
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = $"'{listing.Name}' has been unbanned.";
        }
        return RedirectToAction("Dashboard");
    }
}
