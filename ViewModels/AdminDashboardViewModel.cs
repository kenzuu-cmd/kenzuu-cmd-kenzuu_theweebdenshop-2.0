using TheWeebDenShop.Models;

namespace TheWeebDenShop.ViewModels;

public class AdminDashboardViewModel
{
    public int TotalUsers { get; set; }
    public int TotalStores { get; set; }
    public int TotalListings { get; set; }
    public int BannedListings { get; set; }

    public List<User> Users { get; set; } = new();
    public Dictionary<int, List<string>> UserRoles { get; set; } = new();
    public Dictionary<int, Store> UserStores { get; set; } = new();
    public Dictionary<string, int> StoreListingCounts { get; set; } = new();
    public List<Product> AllUserListings { get; set; } = new();
}
