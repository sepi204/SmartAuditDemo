using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartAuditDemo.Application.DTOs;
using SmartAuditDemo.Services;

namespace SmartAuditDemo.Pages.Organizations;

public class CreateModel : PageModel
{
    private readonly ApiService _apiService;

    public CreateModel(ApiService apiService)
    {
        _apiService = apiService;
    }

    [BindProperty]
    public CreateOrganizationDto Input { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var (success, message, data) = await _apiService.CreateOrganizationAsync(Input);

        if (success)
        {
            TempData["SuccessMessage"] = message;
            return RedirectToPage("./Index");
        }

        TempData["ErrorMessage"] = message;
        return Page();
    }
}



