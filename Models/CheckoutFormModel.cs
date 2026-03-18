using System.ComponentModel.DataAnnotations;

namespace TheWeebDenShop.Models;

/// <summary>
/// View model for the checkout form with shipping and payment info.
/// </summary>
public class CheckoutFormModel
{
    [Required(ErrorMessage = "First name is required.")]
    [StringLength(50)]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(50)]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Street address is required.")]
    [StringLength(200)]
    [Display(Name = "Street Address")]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "City is required.")]
    [StringLength(100)]
    public string City { get; set; } = string.Empty;

    [StringLength(100)]
    [Display(Name = "State/Province")]
    public string? State { get; set; }

    [Required(ErrorMessage = "ZIP/Postal code is required.")]
    [StringLength(20)]
    [Display(Name = "ZIP/Postal Code")]
    public string ZipCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Country is required.")]
    public string Country { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please select a payment method.")]
    [Display(Name = "Payment Method")]
    public string PaymentMethod { get; set; } = "GCash";
}
