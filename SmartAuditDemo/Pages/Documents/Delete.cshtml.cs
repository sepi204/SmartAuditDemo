using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartAuditDemo.Application.DTOs;
using SmartAuditDemo.Services;

namespace SmartAuditDemo.Pages.Documents;

public class DeleteModel : PageModel
{
    private readonly ApiService _apiService;

    public DeleteModel(ApiService apiService)
    {
        _apiService = apiService;
    }

    [BindProperty]
    public Guid Id { get; set; }

    public AccountingDocumentDto? Document { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        Id = id;
        Document = await _apiService.GetDocumentByIdAsync(id);

        if (Document == null)
        {
            TempData["ErrorMessage"] = "سند یافت نشد";
            return RedirectToPage("./Index");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var (success, message) = await _apiService.DeleteDocumentAsync(Id);

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

