using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheWeebDenShop.Models;

/// <summary>
/// Represents a user's manga store. Each registered user can create one store
/// to list and sell manga. The store has a public profile page.
/// </summary>
public class Store
{
    [Key]
    [StringLength(36)]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required(ErrorMessage = "Store name is required.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Store name must be 3–100 characters.")]
    [Display(Name = "Store Name")]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
    [Display(Name = "Store Description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>Path to store logo image in wwwroot/images/stores/</summary>
    [StringLength(300)]
    [Display(Name = "Store Logo")]
    public string? LogoPath { get; set; }

    /// <summary>Path to store banner image in wwwroot/images/stores/</summary>
    [StringLength(300)]
    [Display(Name = "Store Banner")]
    public string? BannerPath { get; set; }

    // ── Owner relationship ──

    [Required]
    public int OwnerId { get; set; }

    [ForeignKey("OwnerId")]
    public User Owner { get; set; } = null!;

    // ── Store listings ──

    public ICollection<Product> Products { get; set; } = new List<Product>();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
