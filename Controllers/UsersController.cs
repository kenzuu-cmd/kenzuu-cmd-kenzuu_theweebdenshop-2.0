using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheWeebDenShop.Models;
using TheWeebDenShop.ViewModels;

namespace TheWeebDenShop.Controllers;

[Authorize(Policy = "RequireAdminRole")]
public class UsersController : Controller
{
    private readonly UserManager<User> _userManager;

    public UsersController(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users.OrderByDescending(u => u.CreatedAt).ToListAsync();
        return View(users);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id.Value);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    public IActionResult Create()
    {
        return View(new UserCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserCreateViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        if (!string.Equals(vm.Password, vm.ConfirmPassword, StringComparison.Ordinal))
        {
            ModelState.AddModelError(nameof(vm.ConfirmPassword), "Passwords do not match.");
            return View(vm);
        }

        var email = vm.Email.Trim();
        var entity = new User
        {
            FullName = vm.FullName.Trim(),
            Email = email,
            UserName = email,
            EmailConfirmed = false,
            CreatedAt = DateTime.UtcNow
        };

        var createResult = await _userManager.CreateAsync(entity, vm.Password);
        if (!createResult.Succeeded)
        {
            foreach (var error in createResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(vm);
        }

        await _userManager.AddToRoleAsync(entity, "Customer");

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id.Value);
        if (user == null)
        {
            return NotFound();
        }

        var vm = new UserEditViewModel
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UserEditViewModel vm)
    {
        if (id != vm.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            return NotFound();
        }

        user.FullName = vm.FullName.Trim();
        user.Email = vm.Email.Trim();
        user.UserName = vm.Email.Trim();

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            foreach (var error in updateResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(vm);
        }

        if (!string.IsNullOrWhiteSpace(vm.NewPassword))
        {
            if (!string.Equals(vm.NewPassword, vm.ConfirmNewPassword, StringComparison.Ordinal))
            {
                ModelState.AddModelError(nameof(vm.ConfirmNewPassword), "New passwords do not match.");
                return View(vm);
            }

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var passwordResult = await _userManager.ResetPasswordAsync(user, resetToken, vm.NewPassword);
            if (!passwordResult.Succeeded)
            {
                foreach (var error in passwordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(vm);
            }
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id.Value);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user != null)
        {
            await _userManager.DeleteAsync(user);
        }

        return RedirectToAction(nameof(Index));
    }
}
