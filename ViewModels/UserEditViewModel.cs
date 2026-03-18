using System.ComponentModel.DataAnnotations;

namespace TheWeebDenShop.ViewModels;

public class UserEditViewModel
{
    [Required]
    public int Id { get; set; }

    [Required]
    [StringLength(120)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [StringLength(100, MinimumLength = 8)]
    [DataType(DataType.Password)]
    public string? NewPassword { get; set; }

    [DataType(DataType.Password)]
    [Compare(nameof(NewPassword), ErrorMessage = "New passwords do not match.")]
    public string? ConfirmNewPassword { get; set; }
}
