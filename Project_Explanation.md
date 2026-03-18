# Project Explanation

This document explains the project in clear, beginner-friendly language while still using correct ASP.NET Core MVC terms.

## 1. What This Project Is

The Weeb Den Shop is an ASP.NET Core MVC web application where users can:

- register and log in,
- browse manga,
- add items to cart and checkout,
- create a store and manage their own manga listings,
- and (if admin) manage users and moderate listings.

Tech stack used in this project:

- ASP.NET Core MVC (Controllers + Razor Views)
- ASP.NET Core Identity (authentication, password hashing, roles)
- Entity Framework Core + SQL Server (data access)
- Session + HttpContext (cart persistence)
- Dependency Injection (services registered in Program.cs)

## 2. Quick File Map (Where to Find Code Fast)

- Controllers: /Controllers
- Entity and form models: /Models
- ViewModels used by views/forms: /ViewModels
- Business logic services: /Services
- EF Core DbContext and DB mapping: /Data/ApplicationDbContext.cs
- Migrations: /Migrations
- Razor UI: /Views
- Static files and uploads: /wwwroot (especially /wwwroot/images)
- App startup + DI + middleware + routing: /Program.cs
- Settings: /appsettings.json and /appsettings.Development.json

## 3. MVC Roles in This Project (Model, View, Controller, ViewModel)

### Model

Models define the data structure used by EF Core and the database.

Examples:

- User, Product, Store, CartItem
- Relationships configured in ApplicationDbContext:
  - one User -> one Store
  - one Store -> many Products

EF Core uses these models to map C# objects to database tables.

### View

Views are Razor (.cshtml) files that render HTML.

- Strongly typed with @model
- Use Tag Helpers like asp-for, asp-validation-for, asp-action, asp-route-\*
- Show form validation messages and TempData results

### Controller

Controllers receive HTTP requests, coordinate logic, and return responses.

- Action methods return View(), RedirectToAction(), LocalRedirect(), etc.
- They call services/DbContext/UserManager/SignInManager
- They enforce security and validation rules before writing data

### ViewModel

ViewModels shape data specifically for a page/form.

Why this matters:

- Avoids overposting (users cannot submit hidden/unexpected entity fields)
- Keeps sensitive/internal fields off forms
- Keeps controllers cleaner and safer

Examples:

- RegisterViewModel, LoginViewModel
- StoreDashboardViewModel, CheckoutViewModel
- UserCreateViewModel, UserEditViewModel

## 4. Validation: Data Annotations + Client/Server Checks

This project uses Data Annotations such as:

- [Required]
- [StringLength]
- [EmailAddress]
- [Compare]
- [Range]
- [Display]
- [DataType]

How validation flows:

1. User enters data in a Razor form.
2. Client-side checks run (via Tag Helpers and validation scripts).
3. Data is posted to a [HttpPost] action.
4. Server-side ModelState validation runs again (critical security layer).
5. If invalid, controller returns the same view with errors.

Important: server-side validation is always the final authority because client-side can be bypassed.

## 5. Routing, Action Methods, and HTTP Attributes

Routing is configured in Program.cs with the default route pattern:

- {controller=Home}/{action=Index}/{id?}

Action methods use attributes to separate read/write behavior:

- [HttpGet] for showing pages/forms
- [HttpPost] for submit operations
- [ValidateAntiForgeryToken] to protect forms from CSRF

Example pattern used throughout:

- GET action returns form view
- POST action validates and processes
- on success, redirect (PRG pattern)

## 6. Authentication and Authorization (Identity-Based)

This project uses ASP.NET Core Identity with custom User entity type.

Configured in Program.cs:

- AddIdentity<User, IdentityRole<int>>()
- Password policy, lockout settings, unique email, confirmed email requirement
- Cookie login/logout/access denied paths
- app.UseAuthentication() and app.UseAuthorization()

### Registration Flow

Main code:

- AccountController.Register (GET/POST)
- RegisterViewModel

How it works:

1. POST receives RegisterViewModel.
2. ModelState checks Data Annotations.
3. UserManager.CreateAsync(user, password) creates user.
4. Identity hashes the password internally and stores PasswordHash.
5. User is assigned Customer role.
6. Email confirmation token is generated and logged link is provided.

### Login Flow

Main code:

- AccountController.Login (GET/POST)

How it works:

1. POST receives LoginViewModel.
2. ModelState is validated.
3. SignInManager.PasswordSignInAsync checks credentials and lockout rules.
4. On success, Identity creates auth cookie and user becomes authenticated.
5. [Authorize] attributes protect pages like cart/checkout/store.

### Password Hashing (No Custom HashingService Here)

There is no separate HashingService class in this codebase.

Instead, hashing is correctly handled by ASP.NET Core Identity via UserManager methods:

- CreateAsync
- ResetPasswordAsync

This is recommended practice because Identity handles secure hashing algorithms, salting, and upgrade paths.

## 7. HttpContext, TempData, and ViewData in Real Use

### HttpContext usage in this project

- CartService uses IHttpContextAccessor to access HttpContext.Session.
- StoreViewController uses HttpContext.Request.Path + QueryString to build a return URL when redirecting unauthenticated users to login.
- HomeController uses HttpContext.TraceIdentifier in Error action diagnostics.

### TempData usage

TempData is used for one-request messages after redirects, for example:

- Success/error messages after checkout
- Newsletter/contact feedback
- Add-to-cart/store management messages
- Email confirmation outcomes

### ViewBag usage

ViewBag is not a primary pattern in this project; strongly typed ViewModels + TempData/ViewData are preferred.

## 8. Services and Dependency Injection (DI)

Registered in Program.cs:

- IProductService -> ProductService
- ICartService -> CartService
- INewsletterService -> NewsletterService
- IImageUploadService -> ImageUploadService

How DI works here:

- Controllers request interfaces in constructors
- ASP.NET Core injects concrete implementations automatically
- This keeps controllers simpler and improves testability/maintainability

Service examples:

- ProductService: EF Core queries, search/filter, featured products
- CartService: session-backed cart JSON serialization
- ImageUploadService: safe upload validation + storage + delete

## 9. CRUD with EF Core (How Data Is Read/Written)

### Store and Manga CRUD

Main code:

- StoreController

Actions:

- Create store: CreateStore
- Create listing: AddManga
- Read listings: Dashboard / StoreViewController.Index
- Update listing: EditManga
- Delete listing: DeleteManga

EF Core operations used:

- Add(), Remove(), SaveChangesAsync()
- Queries with FirstOrDefaultAsync(), AnyAsync(), Include()

### Admin User CRUD

Main code:

- UsersController (protected by [Authorize(Policy = "RequireAdminRole")])

Actions:

- Index, Details, Create, Edit, Delete
- Password reset done safely via GeneratePasswordResetTokenAsync + ResetPasswordAsync

## 10. File Uploads and wwwroot Usage

Main code:

- ImageUploadService

Upload behavior:

- Validates extension and MIME type
- Enforces max size (5 MB)
- Saves file under wwwroot/images/{subfolder}
- Returns relative path for DB storage (example: images/manga/file.jpg)

Why wwwroot matters:

- Only files under wwwroot are directly served as static assets
- Broken path formatting causes missing images even if file exists

Related fix already implemented:

- ProductHtmlHelpers.ResolveImageUrl normalizes image paths across views and supports fallback placeholder

## 11. Migrations and Database Commands

EF Core migration usage in this project:

- db.Database.Migrate() runs at startup to apply pending migrations
- Existing migration files are in /Migrations

Useful commands:

```powershell
dotnet ef migrations add MigrationName
dotnet ef database update
```

Package Manager Console equivalents:

```powershell
Add-Migration MigrationName
Update-Database
```

## 12. Configuration Points

Change these when setting up another machine/environment:

- Connection string: appsettings.json -> ConnectionStrings:DefaultConnection
- Seed admin credentials: SeedAdmin:Email and SeedAdmin:Password
- Identity, session, cookies, routing, policies, DI registrations: Program.cs

## 13. What Breaks If Key Code Is Removed (and How to Restore)

### If authentication middleware is removed

Removed:

- app.UseAuthentication() and/or app.UseAuthorization()

Breaks:

- [Authorize] rules fail to work correctly
- Login state is not applied reliably

Restore:

- Re-add middleware in correct order after UseRouting and before endpoint mapping

### If Identity setup is removed

Removed:

- AddIdentity<User, IdentityRole<int>>()

Breaks:

- Registration/login/password reset flow
- Role-based checks and cookies

Restore:

- Re-register Identity and EntityFramework stores in Program.cs

### If session is removed

Removed:

- AddSession / UseSession / AddHttpContextAccessor

Breaks:

- CartService cannot persist cart in session
- Cart empties unexpectedly or throws null/session errors

Restore:

- Re-enable session services and middleware in Program.cs

### If Data Annotation validation is removed

Breaks:

- Invalid data can enter controllers/DB
- User experience worsens due to missing form guidance

Restore:

- Re-add attributes on ViewModel/Model properties
- Ensure ModelState checks remain in POST actions

### If image helper or upload rules are removed

Breaks:

- Image links may fail (especially uploaded manga under images/manga)
- Unsafe files may be accepted

Restore:

- Reuse ProductHtmlHelpers.ResolveImageUrl for rendering
- Keep ImageUploadService validation and path handling

## 14. Helpful Debugging/Explaining Notes

When explaining a feature, use this order:

1. Route hits controller action
2. Controller validates ModelState and auth
3. Controller calls service or EF Core/UserManager
4. Data is saved/read
5. Controller returns View or Redirect
6. View renders ViewModel + validation messages

Quick checks per feature:

- Registration/Login:
  - verify Identity config in Program.cs
  - verify UserManager/SignInManager calls in AccountController
  - check lockout and email confirmation behavior
- CRUD:
  - verify [HttpPost] + [ValidateAntiForgeryToken]
  - verify SaveChangesAsync is called
  - verify the correct record is queried by id/owner
- Validation:
  - verify Data Annotation attributes on ViewModels
  - verify asp-validation-for and validation summary in Razor
  - verify server-side ModelState checks
- Cart/session:
  - verify AddSession + UseSession
  - inspect session key ShoppingCart
- Uploads/images:
  - verify file exists under wwwroot/images/\*
  - verify stored DB path and rendered URL normalization

Critical pieces that should not be deleted:

- Identity setup + authentication/authorization middleware
- Session setup used by CartService
- Anti-forgery attributes on POST actions
- ModelState checks in POST actions
- EF Core migration/apply flow
- Image path normalization helper

If something fails during demo:

- Check browser network tab for 404/500
- Check server logs for Identity/validation errors
- Check database rows and migration state
- Trace one flow end-to-end: View -> Controller -> Service/EF -> View
