using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartAuditDemo.Application.DTOs;
using SmartAuditDemo.Services;

namespace SmartAuditDemo.Pages.Documents;

public class EditModel : PageModel
{
    private readonly ApiService _apiService;

    public EditModel(ApiService apiService)
    {
        _apiService = apiService;
    }

    [BindProperty]
    public UpdateAccountingDocumentDto Input { get; set; } = new();

    [BindProperty]
    public new IFormFile? File { get; set; }

    public AccountingDocumentDto? Document { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        Document = await _apiService.GetDocumentByIdAsync(id);

        if (Document == null)
        {
            TempData["ErrorMessage"] = "سند یافت نشد";
            return RedirectToPage("./Index");
        }

        Input.Id = Document.Id;
        Input.Title = Document.Title;
        Input.DocumentType = Document.DocumentType;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            Document = await _apiService.GetDocumentByIdAsync(Input.Id);
            return Page();
        }

        var (success, message, data) = await _apiService.UpdateDocumentAsync(Input, File);

        if (success)
        {
            TempData["SuccessMessage"] = message;
            return RedirectToPage("./Index");
        }

        TempData["ErrorMessage"] = message;
        Document = await _apiService.GetDocumentByIdAsync(Input.Id);
        return Page();
    }
}

