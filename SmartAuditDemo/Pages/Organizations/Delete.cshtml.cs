using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartAuditDemo.Application.DTOs;
using SmartAuditDemo.Services;

namespace SmartAuditDemo.Pages.Organizations;

public class DeleteModel : PageModel
{
    private readonly ApiService _apiService;

    public DeleteModel(ApiService apiService)
    {
        _apiService = apiService;
    }

    [BindProperty]
    public Guid Id { get; set; }

    public OrganizationDto? Organization { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        Id = id;
        Organization = await _apiService.GetOrganizationByIdAsync(id);

        if (Organization == null)
        {
            TempData["ErrorMessage"] = "شرکت یافت نشد";
            return RedirectToPage("./Index");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var (success, message) = await _apiService.DeleteOrganizationAsync(Id);

        if (success)
        {
            TempData["SuccessMessage"] = message;
        }
        else
        {
            TempData["ErrorMessage"] = message;
        }

        return RedirectToPage("./Index");
    }
}



