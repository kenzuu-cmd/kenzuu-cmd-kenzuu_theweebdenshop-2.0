using System.ComponentModel.DataAnnotations;

namespace TheWeebDenShop.ViewModels;

public class AdminLoginViewModel
{
    [Required(ErrorMessage = "Admin email is required.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    [Display(Name = "Admin Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }
}
