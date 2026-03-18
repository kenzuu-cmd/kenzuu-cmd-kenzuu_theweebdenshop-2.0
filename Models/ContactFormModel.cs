using System.ComponentModel.DataAnnotations;

namespace TheWeebDenShop.Models;

/// <summary>
/// View model for the contact form with server-side validation.
/// </summary>
public class ContactFormModel
{
    [Required(ErrorMessage = "Please enter your name.")]
    [StringLength(100)]
    [Display(Name = "Full Name")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter a valid email.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Subject")]
    public string? Subject { get; set; }

    [Required(ErrorMessage = "Please enter a message.")]
    [StringLength(2000)]
    [Display(Name = "Message")]
    public string Message { get; set; } = string.Empty;
}
