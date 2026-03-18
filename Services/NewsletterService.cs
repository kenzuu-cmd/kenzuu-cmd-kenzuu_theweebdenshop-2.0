namespace TheWeebDenShop.Services;

/// <summary>
/// Stub newsletter service. Logs the subscription and returns success.
/// Replace with actual email service integration for production.
/// </summary>
public class NewsletterService : INewsletterService
{
    private readonly ILogger<NewsletterService> _logger;

    public NewsletterService(ILogger<NewsletterService> logger)
    {
        _logger = logger;
    }

    public Task<bool> SubscribeAsync(string email)
    {
        // TODO: Integrate with email marketing service (e.g., Mailchimp, SendGrid)
        _logger.LogInformation("Newsletter subscription received for: {Email}", email);
        return Task.FromResult(true);
    }
}
