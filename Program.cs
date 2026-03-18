using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TheWeebDenShop.Data;
using TheWeebDenShop.Models;
using TheWeebDenShop.Services;

var builder = WebApplication.CreateBuilder(args);

// ── Database ──────────────────────────────────────────────────────────
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── Identity ──────────────────────────────────────────────────────────
builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
{
    // Password requirements (strong)
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 4;

    // Lockout after 5 failed attempts
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ── Cookie / auth paths ───────────────────────────────────────────────
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.ExpireTimeSpan = TimeSpan.FromDays(14);
    options.SlidingExpiration = true;
});

// ── MVC ───────────────────────────────────────────────────────────────
builder.Services.AddControllersWithViews();

// ── Application services ──────────────────────────────────────────────
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<INewsletterService, NewsletterService>();
builder.Services.AddScoped<IImageUploadService, ImageUploadService>();

// Session for cart persistence
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddAntiforgery();

// ── Authorization policies ────────────────────────────────────────────
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
});

var app = builder.Build();

// ── Seed roles and admin user on first run ────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
    var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("StartupSeed");
    string[] roles = { "Admin", "Customer" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole<int>(role));
        }
    }

    // Seed admin account only when password is explicitly provided in local config/secrets.
    var userManager = services.GetRequiredService<UserManager<User>>();
    var adminEmail = builder.Configuration["SeedAdmin:Email"] ?? "admin@theweebden.com";
    var adminPassword = builder.Configuration["SeedAdmin:Password"];

    if (await userManager.FindByEmailAsync(adminEmail) == null)
    {
        if (string.IsNullOrWhiteSpace(adminPassword))
        {
            logger.LogWarning("Admin seed skipped. Set SeedAdmin:Password in user secrets or environment variables to create the initial admin user.");
        }

        if (!string.IsNullOrWhiteSpace(adminPassword))
        {
            var admin = new User
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "Admin User",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(admin, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    logger.LogWarning("Admin seed failed: {Code} - {Description}", error.Code, error.Description);
                }
            }
        }
    }
}

// ── Middleware pipeline ───────────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
