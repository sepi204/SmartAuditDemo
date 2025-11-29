using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SmartAuditDemo.Pages.Account;

public class LogoutModel : PageModel
{
    public IActionResult OnGet()
    {
        // TODO: Add logout logic here
        return Page();
    }

    public IActionResult OnPost()
    {
        // TODO: Add logout logic here
        return RedirectToPage("/Account/Login");
    }
}

