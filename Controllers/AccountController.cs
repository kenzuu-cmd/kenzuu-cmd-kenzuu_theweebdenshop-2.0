using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TheWeebDenShop.Models;
using TheWeebDenShop.ViewModels;

namespace TheWeebDenShop.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        SignInManager<User> signInManager,
        UserManager<User> userManager,
        ILogger<AccountController> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
    }

    // ── Login ──

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        var vm = new LoginViewModel { ReturnUrl = returnUrl };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel vm)
    {
        vm.ReturnUrl ??= Url.Content("~/");

        if (!ModelState.IsValid)
            return View(vm);

        var result = await _signInManager.PasswordSignInAsync(
            vm.Email, vm.Password, vm.RememberMe, lockoutOnFailure: true);

        if (result.Succeeded)
            return LocalRedirect(vm.ReturnUrl);

        if (result.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty,
                "Account locked out due to too many failed attempts. Please try again in 15 minutes.");
            return View(vm);
        }

        if (result.IsNotAllowed)
        {
            ModelState.AddModelError(string.Empty,
                "Your email address has not been verified. Please check your inbox and click the confirmation link before logging in.");
            return View(vm);
        }

        ModelState.AddModelError(string.Empty, "Invalid email or password.");
        return View(vm);
    }

    // ── Register ──

    [HttpGet]
    public IActionResult Register(string? returnUrl = null)
    {
        var vm = new RegisterViewModel { ReturnUrl = returnUrl };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel vm)
    {
        vm.ReturnUrl ??= Url.Content("~/");

        if (!ModelState.IsValid)
            return View(vm);

        var user = new User
        {
            UserName = vm.Email,
            Email = vm.Email,
            FullName = $"{vm.FirstName} {vm.LastName}".Trim()
        };

        var result = await _userManager.CreateAsync(user, vm.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "Customer");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Action(
                "ConfirmEmail", "Account",
                new { userId = user.Id, code = token },
                protocol: Request.Scheme);

            _logger.LogWarning("EMAIL CONFIRMATION LINK for {Email}: {Url}", user.Email, callbackUrl);

            vm.RegistrationSucceeded = true;
            return View(vm);
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(vm);
    }

    // ── Confirm Email ──

    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string? userId, string? code)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
            return RedirectToAction("Index", "Home");

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return RedirectToAction("Index", "Home");

        var result = await _userManager.ConfirmEmailAsync(user, code);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = "Your account has been verified. Please log in.";
            return RedirectToAction("Login");
        }

        TempData["ErrorMessage"] = "Email confirmation failed. The link may have expired.";
        return RedirectToAction("Login");
    }

    // ── Logout ──

    [HttpGet]
    public IActionResult Logout()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout(string? _)
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Logout");
    }

    // ── Admin Login ──

    [HttpGet]
    public IActionResult AdminLogin()
    {
        return View(new AdminLoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AdminLogin(AdminLoginViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var result = await _signInManager.PasswordSignInAsync(
            vm.Email, vm.Password, isPersistent: false, lockoutOnFailure: true);

        if (result.Succeeded)
        {
            var user = await _userManager.FindByEmailAsync(vm.Email);
            if (user != null && await _userManager.IsInRoleAsync(user, "Admin"))
            {
                return RedirectToAction("Dashboard", "Admin");
            }

            await _signInManager.SignOutAsync();
            ModelState.AddModelError(string.Empty,
                "This account does not have admin privileges. Please use the regular login.");
            return View(vm);
        }

        if (result.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty,
                "Account locked due to too many failed attempts. Please try again later.");
            return View(vm);
        }

        ModelState.AddModelError(string.Empty, "Invalid admin credentials.");
        return View(vm);
    }

    // ── Access Denied ──

    public IActionResult AccessDenied()
    {
        return View();
    }
}
