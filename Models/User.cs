using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace TheWeebDenShop.Models;

/// <summary>
/// Single source of truth for application users and authentication data.
/// Passwords are stored as hashes in PasswordHash only.
/// </summary>
public class User : IdentityUser<int>
{
    [Required]
    [StringLength(120)]
    public string FullName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Navigation property to the user's store (one-to-one).</summary>
    public Store? Store { get; set; }
}
