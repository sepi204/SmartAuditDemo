using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartAuditDemo.Application.DTOs;
using SmartAuditDemo.Services;

namespace SmartAuditDemo.Pages.Documents;

public class CreateModel : PageModel
{
    private readonly ApiService _apiService;

    public CreateModel(ApiService apiService)
    {
        _apiService = apiService;
    }

    [BindProperty]
    public CreateAccountingDocumentDto Input { get; set; } = new();

    [BindProperty]
    public new IFormFile? File { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (File == null || File.Length == 0)
        {
            ModelState.AddModelError("File", "فایل الزامی است");
            return Page();
        }

        Input.File = File;
        // TODO: Get from authentication context
        Input.UploaderUserId = Guid.NewGuid();

        var (success, message, data) = await _apiService.CreateDocumentAsync(Input, File);

        if (success)
        {
            TempData["SuccessMessage"] = message;
            return RedirectToPage("./Index");
        }

        TempData["ErrorMessage"] = message;
        return Page();
    }
}

