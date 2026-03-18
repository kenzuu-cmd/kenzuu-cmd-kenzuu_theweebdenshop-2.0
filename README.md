# The Weeb Den Shop

The Weeb Den Shop is an ASP.NET Core MVC (.NET 8) e-commerce school project focused on manga products, user accounts, cart/checkout flow, and role-based admin/store features.

## Project Goals

- Demonstrate a full MVC architecture with clear separation of concerns.
- Implement secure authentication and authorization with ASP.NET Core Identity.
- Support core commerce behavior: browse, filter, cart, and checkout.
- Show CRUD operations through user management and store-owned listings.
- Use SQL Server LocalDB for local development and reproducible setup.

## Tech Stack

- .NET 8
- ASP.NET Core MVC + Razor Views
- Entity Framework Core (SQL Server provider)
- ASP.NET Core Identity (hashed passwords, roles, lockout, email confirmation)
- SQL Server Express LocalDB
- Session-based cart persistence

## Main Features

- Account registration, login, logout, and email confirmation flow.
- Password security through Identity hashing (no plaintext password storage).
- Role model: `Admin` and `Customer`.
- Product catalog with search and filters.
- Cart management (add, update quantity, remove, clear).
- Demo checkout workflow with form validation.
- Store module: each user can create one store and manage their own listings.
- Admin dashboard: user/store statistics and listing ban/unban controls.
- Admin user CRUD screens.
- Secure image upload service for store logos and manga covers.

## Folder Structure

Key folders and what they contain:

- `Controllers/`: HTTP endpoints and page flow logic.
- `Models/`: EF entities and form models.
- `ViewModels/`: UI-specific models passed from controllers to views.
- `Views/`: Razor pages and shared UI partials.
- `Services/`: business logic (`ProductService`, `CartService`, `ImageUploadService`, `NewsletterService`).
- `Data/`: `ApplicationDbContext` and database mapping/seeding.
- `Migrations/`: EF Core migration history.
- `wwwroot/`: static assets (CSS, JS, images).
- `Program.cs`: dependency injection, Identity options, policies, middleware pipeline, startup seeding.

## Local Requirements

- .NET 8 SDK
- SQL Server LocalDB (usually installed with Visual Studio)
- Optional: EF Core CLI tools for migration commands

```bash
dotnet tool install --global dotnet-ef
```

## Configuration

Default LocalDB connection string in `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=WeebDenDb;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

Admin seeding is supported but requires setting a password in secrets or environment variables.

## Run Locally (LocalDB)

1. Restore dependencies.

```bash
dotnet restore
```

2. Apply migrations (creates/updates LocalDB database).

```bash
dotnet ef database update
```

3. Configure optional seed admin credentials.

```bash
dotnet user-secrets init
dotnet user-secrets set "SeedAdmin:Email" "admin@theweebden.com"
dotnet user-secrets set "SeedAdmin:Password" "YourStrongAdminPassword123!"
```

4. Run the app.

```bash
dotnet run
```

5. Open the URL shown in terminal output.

## Authentication and Security Notes

- Identity enforces password policy, lockout, and unique email settings in `Program.cs`.
- Login and register are handled in `AccountController`.
- Password hashing is automatic via `UserManager.CreateAsync` and `UserManager.ResetPasswordAsync`.
- Protected modules use `[Authorize]` and admin features use policy `RequireAdminRole`.
- Forms use anti-forgery validation (`[ValidateAntiForgeryToken]`).

## Data Model Highlights

- `User`: Identity user (int key), with `FullName` and optional one-to-one `Store`.
- `Store`: one store per owner, with optional logo/banner paths.
- `Product`: platform-seeded products + user store listings, moderation fields (`IsApproved`, `IsBanned`).
- `CartItem`: session snapshot used by cart service.

## Cart Product Image Fix (Applied)

### Problem

Cart and checkout images were rendered using only the image file name:

- Old behavior: `~/images/@Path.GetFileName(item.Image)`

This fails for uploaded store manga because uploads are saved under subfolders like:

- `images/manga/<guid>.jpg`

Dropping `manga/` caused broken image URLs on cart-related pages.

### Solution

- Added centralized helper: `ProductHtmlHelpers.ResolveImageUrl(string? imagePath)`.
- Replaced ad-hoc image path logic across cart and product listing views.
- Added fallback behavior with `onerror` to `placeholder.svg`.

### Why this works

`ResolveImageUrl` normalizes all known formats:

- `images/filename.jpg`
- `/images/filename.jpg`
- `images/manga/filename.jpg`
- plain `filename.jpg`
- absolute `http/https` URLs

If invalid or empty, it returns `/images/placeholder.svg`.

### If images break again, check these first

1. `item.Image` / `product.Image` values in DB or session JSON.
2. Physical file presence under `wwwroot/images/...`.
3. Whether path includes subfolder (`images/manga/...`) and is preserved.
4. Whether static files middleware is enabled (`app.UseStaticFiles()`).
5. Browser network tab for 404 URL details.
6. Any direct path concatenation in views that bypasses `ResolveImageUrl`.

## Known Limitations

- Checkout is demo-only (no payment gateway integration).
- Newsletter service currently logs subscriptions (stub implementation).
- Contact form is non-persistent.
- Admin dashboard/user list pages are not paginated yet.

## Suggested Verification Checklist

1. Register a user and confirm email.
2. Login and browse catalog with filters.
3. Add seeded products and store-uploaded products to cart.
4. Verify images in Products, StoreView, Cart, and Checkout.
5. Perform checkout and confirm cart clears.
6. Create/edit/delete store manga entries.
7. Login as admin and test listing moderation + user CRUD.
