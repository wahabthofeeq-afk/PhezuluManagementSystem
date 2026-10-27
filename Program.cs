using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PhezuluManagementSystem.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "DefaultConnection was not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;

    options.Password.RequiredLength = 10;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();


await using (var scope = app.Services.CreateAsyncScope())
{
    var services = scope.ServiceProvider;

    var roleManager =
        services.GetRequiredService<RoleManager<IdentityRole>>();

    var userManager =
        services.GetRequiredService<UserManager<IdentityUser>>();

    string[] roles =
    {
        "Owner",
        "Administrator",
        "QuotationStaff",
        "ManufacturingStaff",
        "InstallationStaff",
        "ReadOnly"
    };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            var result = await roleManager.CreateAsync(
                new IdentityRole(role));

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Failed to create role {role}: " +
                    string.Join("; ",
                        result.Errors.Select(e => e.Description)));
            }
        }
    }

    var ownerEmail =
        app.Configuration["BootstrapOwner:Email"];

    var ownerPassword =
        app.Configuration["BootstrapOwner:Password"];

    if (!string.IsNullOrWhiteSpace(ownerEmail) &&
        !string.IsNullOrWhiteSpace(ownerPassword))
    {
        var owner = await userManager.FindByEmailAsync(ownerEmail);

        if (owner == null)
        {
            owner = new IdentityUser
            {
                UserName = ownerEmail,
                Email = ownerEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(
                owner, ownerPassword);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    "Failed to create owner: " +
                    string.Join("; ",
                        result.Errors.Select(e => e.Description)));
            }
        }

        if (!await userManager.IsInRoleAsync(owner, "Owner"))
        {
            var result = await userManager.AddToRoleAsync(
                owner, "Owner");

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    "Failed to assign Owner role: " +
                    string.Join("; ",
                        result.Errors.Select(e => e.Description)));
            }
        }
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.MapRazorPages();

app.Run();