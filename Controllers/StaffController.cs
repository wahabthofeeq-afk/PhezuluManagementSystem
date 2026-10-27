using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PhezuluManagementSystem.ViewModels;

namespace PhezuluManagementSystem.Controllers;

[Authorize(Roles = "Owner")]
public class StaffController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    private static readonly string[] AllowedStaffRoles =
    {
        "Administrator",
        "QuotationStaff",
        "ManufacturingStaff",
        "InstallationStaff",
        "ReadOnly"
    };

    public StaffController(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var users = _userManager.Users
            .OrderBy(u => u.Email)
            .ToList();

        var staff = new List<StaffListItem>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            staff.Add(new StaffListItem
            {
                Id = user.Id,
                Email = user.Email ?? user.UserName ?? "",
                Roles = string.Join(", ", roles),
                IsOwner = roles.Contains("Owner")
            });
        }

        return View(staff);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await LoadRolesAsync();
        return View(new CreateStaffViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateStaffViewModel model)
    {
        if (!AllowedStaffRoles.Contains(model.Role))
        {
            ModelState.AddModelError(
                nameof(model.Role), "Select a valid staff role.");
        }
        else if (!await _roleManager.RoleExistsAsync(model.Role))
        {
            ModelState.AddModelError(
                nameof(model.Role), "The selected role does not exist.");
        }

        if (!ModelState.IsValid)
        {
            await LoadRolesAsync();
            return View(model);
        }

        var email = model.Email.Trim();
        var existingUser = await _userManager.FindByEmailAsync(email);

        if (existingUser != null)
        {
            ModelState.AddModelError(
                nameof(model.Email),
                "An account with this email already exists.");

            await LoadRolesAsync();
            return View(model);
        }

        var user = new IdentityUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = false
        };

        var createResult = await _userManager.CreateAsync(
            user, model.Password);

        if (!createResult.Succeeded)
        {
            foreach (var error in createResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            await LoadRolesAsync();
            return View(model);
        }

        var roleResult = await _userManager.AddToRoleAsync(
            user, model.Role);

        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);

            foreach (var error in roleResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            await LoadRolesAsync();
            return View(model);
        }

        TempData["SuccessMessage"] =
            $"Staff account created for {email}.";

        return RedirectToAction(nameof(Index));
    }

    private async Task LoadRolesAsync()
    {
        var roles = new List<SelectListItem>();

        foreach (var role in AllowedStaffRoles)
        {
            if (await _roleManager.RoleExistsAsync(role))
            {
                roles.Add(new SelectListItem(role, role));
            }
        }

        ViewBag.Roles = roles;
    }
}

public class StaffListItem
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Roles { get; set; } = string.Empty;
    public bool IsOwner { get; set; }
}