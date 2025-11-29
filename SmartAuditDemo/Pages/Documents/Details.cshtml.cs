using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartAuditDemo.Application.DTOs;
using SmartAuditDemo.Services;

namespace SmartAuditDemo.Pages.Documents;

public class DetailsModel : PageModel
{
    private readonly ApiService _apiService;

    public DetailsModel(ApiService apiService)
    {
        _apiService = apiService;
    }

    public AccountingDocumentDto? Document { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        Document = await _apiService.GetDocumentByIdAsync(id);

        if (Document == null)
        {
            TempData["ErrorMessage"] = "سند یافت نشد";
            return RedirectToPage("./Index");
        }

        return Page();
    }
}

