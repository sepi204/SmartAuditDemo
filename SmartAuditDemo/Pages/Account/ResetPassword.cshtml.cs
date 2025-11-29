using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SmartAuditDemo.Pages.Account;

public class ResetPasswordModel : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Token { get; set; } = string.Empty;

        [Required(ErrorMessage = "رمز عبور الزامی است")]
        [StringLength(100, ErrorMessage = "رمز عبور باید حداقل {2} کاراکتر باشد", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "رمز عبور و تکرار آن مطابقت ندارند")]
        [Display(Name = "تکرار رمز عبور")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public IActionResult OnGet(string? token, string? email)
    {
        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
        {
            return RedirectToPage("./ForgotPassword");
        }

        Input.Token = token;
        Input.Email = email;
        return Page();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // TODO: Add password reset logic here
        TempData["Message"] = "رمز عبور شما با موفقیت تغییر یافت.";
        return RedirectToPage("./Login");
    }
}

