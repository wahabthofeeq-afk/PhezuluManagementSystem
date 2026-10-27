
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PhezuluManagementSystem.Areas.Identity.Pages.Account;

public class RegisterModel : PageModel
{
    public IActionResult OnGet()
    {
        return RedirectToPage("/Account/Login");
    }

    public IActionResult OnPost()
    {
        return RedirectToPage("/Account/Login");
    }
}
