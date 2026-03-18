using TheWeebDenShop.Models;

namespace TheWeebDenShop.Services;

/// <summary>
/// Provides access to the product catalog.
/// Backed by EF Core queries in the current implementation.
/// </summary>
public interface IProductService
{
    List<Product> GetAll();
    Product? GetById(string id);
    List<Product> GetByGenre(string genre);
    List<Product> Search(string? searchTerm, string? genre, decimal? minPrice, decimal? maxPrice);
    List<string> GetGenres();
    List<Product> GetFeatured(int count = 8);
}
