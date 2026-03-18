using TheWeebDenShop.Models;

namespace TheWeebDenShop.ViewModels;

public class ContactViewModel
{
    public ContactFormModel ContactForm { get; set; } = new();
    public string? SuccessMessage { get; set; }
}
