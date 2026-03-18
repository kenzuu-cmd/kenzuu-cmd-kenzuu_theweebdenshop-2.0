using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheWeebDenShop.Models;

/// <summary>
/// Represents a manga product in the catalog.
/// Products may be seeded (platform catalog) or user-created (via a Store).
/// When StoreId is set, the manga belongs to a user's store.
/// </summary>
public class Product
{
    [Key]
    [StringLength(36)]
    public string Id { get; set; } = string.Empty;

    [Required(ErrorMessage = "Title is required.")]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
    [Display(Name = "Title")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Price is required.")]
    [Range(0.01, 999999.99, ErrorMessage = "Price must be between ₱0.01 and ₱999,999.99.")]
    [Column(TypeName = "decimal(10,2)")]
    [Display(Name = "Price (₱)")]
    public decimal Price { get; set; }

    [StringLength(500, ErrorMessage = "Short description cannot exceed 500 characters.")]
    [Display(Name = "Short Description")]
    public string Short { get; set; } = string.Empty;

    [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
    [Display(Name = "Full Description")]
    public string Description { get; set; } = string.Empty;

    [StringLength(300)]
    [Display(Name = "Cover Image")]
    public string Image { get; set; } = string.Empty;

    [Required(ErrorMessage = "Author is required.")]
    [StringLength(200, ErrorMessage = "Author name cannot exceed 200 characters.")]
    [Display(Name = "Author")]
    public string Author { get; set; } = string.Empty;

    [Required(ErrorMessage = "Genre is required.")]
    [StringLength(100, ErrorMessage = "Genre cannot exceed 100 characters.")]
    [Display(Name = "Genre")]
    public string Genre { get; set; } = string.Empty;

    [Range(1, 1000, ErrorMessage = "Volumes must be between 1 and 1000.")]
    [Display(Name = "Volumes")]
    public int Volumes { get; set; }

    [Range(0, 99999, ErrorMessage = "Stock must be between 0 and 99,999.")]
    [Display(Name = "Stock")]
    public int Stock { get; set; }

    [Range(0.0, 5.0, ErrorMessage = "Rating must be between 0 and 5.")]
    public double Rating { get; set; }

    // ── Store ownership (nullable — seeded products have no store) ──

    [StringLength(36)]
    public string? StoreId { get; set; }

    [ForeignKey("StoreId")]
    public Store? Store { get; set; }

    /// <summary>Whether this listing is approved by admin (for moderation).</summary>
    public bool IsApproved { get; set; } = true;

    /// <summary>Whether this listing has been banned/removed by admin.</summary>
    public bool IsBanned { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
