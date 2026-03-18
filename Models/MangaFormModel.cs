using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace TheWeebDenShop.Models;

/// <summary>
/// Form model for creating or editing a manga listing.
/// Includes an IFormFile for cover image upload with validation.
/// </summary>
public class MangaFormModel
{
    /// <summary>Set when editing an existing manga.</summary>
    public string? Id { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Title must be 2–200 characters.")]
    [Display(Name = "Manga Title")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Author is required.")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Author must be 2–200 characters.")]
    [Display(Name = "Author")]
    public string Author { get; set; } = string.Empty;

    [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
    [Display(Name = "Description")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Genre is required.")]
    [StringLength(100)]
    [Display(Name = "Genre")]
    public string Genre { get; set; } = string.Empty;

    [Required(ErrorMessage = "Price is required.")]
    [Range(0.01, 999999.99, ErrorMessage = "Price must be between ₱0.01 and ₱999,999.99.")]
    [Display(Name = "Price (₱)")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Stock quantity is required.")]
    [Range(0, 99999, ErrorMessage = "Stock must be between 0 and 99,999.")]
    [Display(Name = "Stock")]
    public int Stock { get; set; }

    [Range(1, 1000, ErrorMessage = "Volumes must be between 1 and 1000.")]
    [Display(Name = "Number of Volumes")]
    public int Volumes { get; set; } = 1;

    /// <summary>Cover image upload. Required for new listings, optional when editing.</summary>
    [Display(Name = "Cover Image")]
    public IFormFile? CoverImage { get; set; }

    /// <summary>Existing image path (used during edits to preserve current image).</summary>
    public string? ExistingImagePath { get; set; }
}
