# Project Explanation (Beginner + Teacher Guide)

This file explains the project in plain language and maps features to common school requirements.

## 1. What This Project Is

The Weeb Den Shop is a web app where users can:

- create an account,
- log in,
- browse manga,
- add items to a cart,
- checkout,
- and (if they own a store) add/edit/delete their own manga listings.

Admins can manage users and ban/unban store listings.

## 2. School Requirement Mapping

### Requirement: Registration

- Code location:
  - `Controllers/AccountController.cs` (`Register` GET/POST)
  - `Views/Account/Register.cshtml`
  - `ViewModels/RegisterViewModel.cs`
- How it works:
  - User submits registration form.
  - Server validates fields.
  - `UserManager.CreateAsync(user, password)` creates user.
  - User is assigned `Customer` role.

If removed/broken:

- New users cannot create accounts.
- Login only works for already existing users.

Teacher debug notes:

- Check `ModelState` errors in POST Register.
- Check Identity password policy errors.
- Check if `Customer` role exists (seeded in `Program.cs`).

### Requirement: Login / Authentication

- Code location:
  - `Controllers/AccountController.cs` (`Login` GET/POST)
  - `Views/Account/Login.cshtml`
  - `Program.cs` cookie + Identity config
- How it works:
  - User submits email/password.
  - `PasswordSignInAsync` checks credentials.
  - If valid, auth cookie is created.
  - `[Authorize]` protects cart/checkout/store routes.

If removed/broken:

- Protected pages may become inaccessible or unprotected.
- Cart/checkout/store access flow breaks.

Teacher debug notes:

- Verify `app.UseAuthentication()` and `app.UseAuthorization()` exist.
- Check login path config in `Program.cs`.
- Check whether account is locked or email is unconfirmed.

### Requirement: Password Hashing

- Code location:
  - `Program.cs` Identity setup
  - `Controllers/AccountController.cs`
  - `Controllers/UsersController.cs` (password reset flow)
- How it works:
  - Password is never stored as plain text.
  - Identity hashes password before saving.

If removed/broken:

- Security requirement fails.
- User auth becomes insecure or nonfunctional.

Teacher debug notes:

- Confirm code uses `UserManager` methods (`CreateAsync`, `ResetPasswordAsync`).
- Confirm there is no manual `Password` field in DB model.

### Requirement: CRUD

- Code location:
  - Store listing CRUD: `Controllers/StoreController.cs`
    - Create: `AddManga`
    - Read: `Dashboard` / public `StoreViewController.Index`
    - Update: `EditManga`
    - Delete: `DeleteManga`
  - User CRUD (admin): `Controllers/UsersController.cs`
- How it works:
  - Forms submit data to controller actions.
  - Controller validates and updates database through EF Core.
  - Views show updated data.

If removed/broken:

- Cannot add/edit/delete listings or users.
- Database data becomes stale because updates do not execute.

Teacher debug notes:

- Confirm HTTP verb attributes are correct (`[HttpPost]`).
- Confirm anti-forgery token is present and validated.
- Confirm `ApplicationDbContext` is registered and migration is applied.

### Requirement: At Least Two Models

- Included models:
  - `User`, `Store`, `Product`, `CartItem` (and form models)
- Relationship examples:
  - one user -> one store,
  - one store -> many products.

### Requirement: MVC Pattern

- Model: entities and validation rules (`Models/*`)
- View: Razor pages (`Views/*`)
- Controller: request/response logic (`Controllers/*`)
- Service layer: reusable business logic (`Services/*`)

## 3. How Main Features Work

### A. Product Browsing and Search

- `ProductsController.Index` calls `ProductService.Search(...)`.
- Filters include search term, genre, and price range.
- UI is rendered in `Views/Products/Index.cshtml` and `Views/Shared/_ProductCard.cshtml`.

If broken:

- Product list may appear empty or filters do nothing.

Debug checklist:

- Check `ProductService.Search` where conditions.
- Check if products are `IsApproved` and not `IsBanned`.
- Check migration/seed data exists.

### B. Cart and Checkout

- Add to cart: `ProductsController.AddToCart` or `StoreViewController.AddToCart`.
- Cart storage: `Services/CartService.cs` using session JSON.
- Cart display: `Views/Cart/Index.cshtml`.
- Checkout display + form validation: `Views/Checkout/Index.cshtml`.

If broken:

- Items may not persist between requests.
- Quantity update/remove may fail.

Debug checklist:

- Ensure session services are enabled in `Program.cs`.
- Ensure `app.UseSession()` is in middleware pipeline.
- Inspect session key `ShoppingCart`.

### C. Store Management (Seller Features)

- User creates one store (`CreateStore`).
- User uploads manga cover image and listing data (`AddManga`).
- User edits listing and optionally replaces image (`EditManga`).

If broken:

- Seller cannot manage listings.
- Uploaded images may fail due to validation.

Debug checklist:

- Check `ImageUploadService` extension, MIME type, and file size rules.
- Check folder write permissions under `wwwroot/images/manga`.
- Check ModelState validation errors on form submit.

### D. Admin Features

- Admin dashboard: `AdminController.Dashboard`.
- Listing moderation: `BanListing` / `UnbanListing`.
- User CRUD: `UsersController`.

If broken:

- Admin cannot manage users/listings.

Debug checklist:

- Check role assignment (`Admin`).
- Check policy protection `[Authorize(Policy = "RequireAdminRole")]`.
- Check seeded admin user and password configuration.

## 4. Cart Image Bug Fix (Important)

### Original issue

Cart and checkout pages forced image URLs into this format:

- `/images/<filename>`

But uploaded store images are saved as:

- `images/manga/<filename>`

So cart pages removed `manga/` and produced broken image links.

### What was changed

- Added a shared helper: `ProductHtmlHelpers.ResolveImageUrl(string? imagePath)`.
- Updated image rendering in cart, checkout, and product listing/detail views to use this helper.
- Added image fallback: `onerror -> /images/placeholder.svg`.

### Why this fix is correct

The helper normalizes all known path formats consistently:

- `images/file.jpg`
- `/images/file.jpg`
- `images/manga/file.jpg`
- plain file name
- full `http/https` URL

So every page now builds image URLs the same way.

### If someone removes this helper

Expected symptoms:

- Cart or checkout images disappear for store-uploaded products.
- Some pages work while others fail (inconsistent logic).

What to inspect:

- All views that show images.
- Any direct usage of `Path.GetFileName(image)`.
- Whether path-subfolder information is lost.

## 5. What Happens If Key Code Is Removed

### If `UseAuthentication` / `UseAuthorization` is removed

- Login state may not be enforced.
- `[Authorize]` no longer protects routes correctly.

### If `AddIdentity` setup is removed

- Registration/login fails.
- Password hashing/authentication pipeline breaks.

### If `AddSession` or `UseSession` is removed

- Cart no longer persists in session.
- Cart can reset unexpectedly each request.

### If `db.Database.Migrate()` is removed at startup

- Fresh machines may fail on first run due to missing DB schema.

### If anti-forgery attributes are removed

- Security requirement weakens.
- Forms become vulnerable to CSRF.

## 6. Teacher Demo Script (Quick)

1. Run app and open Home.
2. Register a new user.
3. Confirm email via logged confirmation link.
4. Login and add products to cart.
5. Verify cart images load for both seeded and uploaded products.
6. Create store and add manga with image upload.
7. Edit manga and verify image replacement.
8. Login as admin and ban/unban listing.
9. Open Users page and perform CRUD action.

## 7. Practical Debugging Tips for Oral Defense

- Always start from route -> controller action -> service -> view.
- If UI is broken, inspect browser Network tab (404/500 errors).
- If data is missing, verify database values and migration state.
- If validation blocks submission, inspect `ModelState` errors.
- If authorization fails, check user roles and policy attributes.
- If image is broken, print/log stored path and compare with actual `wwwroot/images` file.

## 8. Short Summary for Presentation

This project is a complete MVC web app with Identity auth, hashed passwords, role-based access, EF Core data, and CRUD operations. It includes both customer and admin workflows, plus a fixed and centralized image path system so product images display consistently across the catalog, cart, and checkout pages.
