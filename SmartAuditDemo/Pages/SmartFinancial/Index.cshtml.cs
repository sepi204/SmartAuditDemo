using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartAuditDemo.Application.DTOs;
using SmartAuditDemo.Services;

namespace SmartAuditDemo.Pages.SmartFinancial;

public class IndexModel : PageModel
{
    private readonly ApiService _apiService;

    public IndexModel(ApiService apiService)
    {
        _apiService = apiService;
    }

    public List<AccountingDocumentDto>? Documents { get; set; }

    public async Task OnGetAsync()
    {
        var filter = new AccountingDocumentFilterDto
        {
            PageNumber = 1,
            PageSize = 100 // دریافت همه اسناد برای dropdown
        };

        var result = await _apiService.GetDocumentsAsync(filter);
        Documents = result?.Data ?? new List<AccountingDocumentDto>();
    }

    public async Task<IActionResult> OnPostGenerateAsync(Guid documentId)
    {
        if (documentId == Guid.Empty)
        {
            TempData["ErrorMessage"] = "لطفاً یک سند را انتخاب کنید";
            return RedirectToPage();
        }

        var (success, message, data, fileName) = await _apiService.GenerateFinancialStatementAsync(documentId);

        if (success && data != null && !string.IsNullOrEmpty(fileName))
        {
            return File(data, 
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                fileName);
        }

        TempData["ErrorMessage"] = message;
        return RedirectToPage();
    }
}



