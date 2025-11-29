using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartAuditDemo.Application.DTOs;
using SmartAuditDemo.Services;

namespace SmartAuditDemo.Pages.Organizations;

public class EditModel : PageModel
{
    private readonly ApiService _apiService;

    public EditModel(ApiService apiService)
    {
        _apiService = apiService;
    }

    [BindProperty]
    public UpdateOrganizationDto Input { get; set; } = new();

    public OrganizationDto? Organization { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        Organization = await _apiService.GetOrganizationByIdAsync(id);

        if (Organization == null)
        {
            TempData["ErrorMessage"] = "شرکت یافت نشد";
            return RedirectToPage("./Index");
        }

        Input.Id = Organization.Id;
        Input.Name = Organization.Name;
        Input.NationalId = Organization.NationalId;
        Input.RegistrationNumber = Organization.RegistrationNumber;
        Input.EconomicCode = Organization.EconomicCode;
        Input.CompanyType = Organization.CompanyType;
        Input.BusinessField = Organization.BusinessField;
        Input.IsActive = Organization.IsActive;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            Organization = await _apiService.GetOrganizationByIdAsync(Input.Id);
            return Page();
        }

        var (success, message, data) = await _apiService.UpdateOrganizationAsync(Input);

        if (success)
        {
            TempData["SuccessMessage"] = message;
            return RedirectToPage("./Index");
        }

        TempData["ErrorMessage"] = message;
        Organization = await _apiService.GetOrganizationByIdAsync(Input.Id);
        return Page();
    }
}



