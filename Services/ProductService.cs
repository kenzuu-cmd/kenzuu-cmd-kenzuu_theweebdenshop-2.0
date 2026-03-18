using Microsoft.EntityFrameworkCore;
using TheWeebDenShop.Data;
using TheWeebDenShop.Models;

namespace TheWeebDenShop.Services;

/// <summary>
/// EF Core-backed product service. Reads from the Products table in SQL Server.
/// Filters out banned listings from public queries. User-created manga with
/// IsApproved=true and IsBanned=false appear alongside seeded products.
/// </summary>
public class ProductService : IProductService
{
    private readonly ApplicationDbContext _db;
    private static readonly Random _random = new();

    public ProductService(ApplicationDbContext db)
    {
        _db = db;
    }

    /// <summary>Returns all visible (approved, non-banned) products.</summary>
    public List<Product> GetAll() =>
        _db.Products.AsNoTracking()
            .Where(p => p.IsApproved && !p.IsBanned)
            .ToList();

    public Product? GetById(string id) =>
        _db.Products.AsNoTracking().FirstOrDefault(p => p.Id == id);

    public List<Product> GetByGenre(string genre) =>
        _db.Products.AsNoTracking()
            .Where(p => p.Genre == genre && p.IsApproved && !p.IsBanned)
            .ToList();

    public List<string> GetGenres() =>
        _db.Products.AsNoTracking()
            .Where(p => p.IsApproved && !p.IsBanned)
            .Select(p => p.Genre)
            .Distinct()
            .OrderBy(g => g)
            .ToList();

    public List<Product> GetFeatured(int count = 8)
    {
        var all = _db.Products.AsNoTracking()
            .Where(p => p.IsApproved && !p.IsBanned)
            .ToList();
        return all.OrderBy(_ => _random.Next()).Take(count).ToList();
    }

    public List<Product> Search(string? searchTerm, string? genre, decimal? minPrice, decimal? maxPrice)
    {
        var query = _db.Products.AsNoTracking()
            .Where(p => p.IsApproved && !p.IsBanned)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(term) ||
                p.Author.ToLower().Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(genre))
        {
            query = query.Where(p => p.Genre == genre);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(p => p.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= maxPrice.Value);
        }

        return query.ToList();
    }
}
