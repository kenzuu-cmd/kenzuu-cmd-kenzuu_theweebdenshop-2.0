namespace TheWeebDenShop.Services;

/// <summary>
/// Handles newsletter signup requests.
/// Extension point: Integrate with Mailchimp, SendGrid, or store in database.
/// </summary>
public interface INewsletterService
{
    Task<bool> SubscribeAsync(string email);
}
