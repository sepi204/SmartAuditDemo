using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartAuditDemo.Application.DTOs;
using SmartAuditDemo.Services;

namespace SmartAuditDemo.Pages.Organizations;

public class DetailsModel : PageModel
{
    private readonly ApiService _apiService;

    public DetailsModel(ApiService apiService)
    {
        _apiService = apiService;
    }

    public OrganizationDto? Organization { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        Organization = await _apiService.GetOrganizationByIdAsync(id);

        if (Organization == null)
        {
            TempData["ErrorMessage"] = "شرکت یافت نشد";
            return RedirectToPage("./Index");
        }

        return Page();
    }
}

