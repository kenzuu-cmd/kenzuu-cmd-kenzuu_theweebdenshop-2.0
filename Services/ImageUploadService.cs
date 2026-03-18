using Microsoft.AspNetCore.Http;

namespace TheWeebDenShop.Services;

/// <summary>
/// Handles secure file uploads for images (manga covers, store logos/banners).
/// Files are stored in wwwroot/images/ subdirectories; paths are tracked in the DB.
/// 
/// Security measures:
/// - Whitelist of allowed extensions (.jpg, .jpeg, .png, .webp, .gif)
/// - File size limit (5 MB)
/// - Sanitized file names (GUID-based to prevent path traversal)
/// - Content-type validation
/// </summary>
public interface IImageUploadService
{
    /// <summary>
    /// Saves an uploaded image to wwwroot/images/{subfolder}/.
    /// Returns the relative path (e.g., "images/manga/abc123.jpg") or null on failure.
    /// </summary>
    Task<string?> SaveImageAsync(IFormFile file, string subfolder);

    /// <summary>Deletes an image from wwwroot by its relative path.</summary>
    void DeleteImage(string? relativePath);
}

public class ImageUploadService : IImageUploadService
{
    private readonly IWebHostEnvironment _env;

    // Security: Only allow safe image extensions
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp", ".gif"
    };

    // Security: Validate content type matches an image MIME type
    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg", "image/png", "image/webp", "image/gif"
    };

    private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

    public ImageUploadService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<string?> SaveImageAsync(IFormFile file, string subfolder)
    {
        if (file == null || file.Length == 0)
            return null;

        // Security: Enforce file size limit
        if (file.Length > MaxFileSizeBytes)
            return null;

        // Security: Validate extension
        var extension = Path.GetExtension(file.FileName);
        if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension))
            return null;

        // Security: Validate content type
        if (!AllowedContentTypes.Contains(file.ContentType))
            return null;

        // Security: Generate a safe GUID-based filename to prevent path traversal
        var safeFileName = $"{Guid.NewGuid()}{extension.ToLowerInvariant()}";
        var uploadDir = Path.Combine(_env.WebRootPath, "images", subfolder);

        Directory.CreateDirectory(uploadDir);

        var filePath = Path.Combine(uploadDir, safeFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Return relative path for storage in DB (used with ~/ in Razor)
        return $"images/{subfolder}/{safeFileName}";
    }

    public void DeleteImage(string? relativePath)
    {
        if (string.IsNullOrEmpty(relativePath)) return;

        var fullPath = Path.Combine(_env.WebRootPath, relativePath.Replace('/', Path.DirectorySeparatorChar));
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }
}
