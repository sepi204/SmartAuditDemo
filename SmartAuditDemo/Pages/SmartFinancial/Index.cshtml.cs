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

    public async Task<IActionResult> OnPostGenerateAsync(Guid? documentId, IFormFile? uploadedFile)
    {
        // اگر فایل آپلود شده باشد، از آن استفاده کن
        if (uploadedFile != null && uploadedFile.Length > 0)
        {
            var (success, message, data, fileName) = await _apiService.GenerateFinancialStatementFromUploadAsync(uploadedFile);

            if (success && data != null && !string.IsNullOrEmpty(fileName))
            {
                return File(data, 
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                    fileName);
            }

            TempData["ErrorMessage"] = message;
            return RedirectToPage();
        }

        // در غیر این صورت از سند انتخاب شده استفاده کن
        if (!documentId.HasValue || documentId.Value == Guid.Empty)
        {
            TempData["ErrorMessage"] = "لطفاً یک سند را انتخاب کنید یا فایلی را آپلود کنید";
            return RedirectToPage();
        }

        var (success2, message2, data2, fileName2) = await _apiService.GenerateFinancialStatementAsync(documentId.Value);

        if (success2 && data2 != null && !string.IsNullOrEmpty(fileName2))
        {
            return File(data2, 
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                fileName2);
        }

        TempData["ErrorMessage"] = message2;
        return RedirectToPage();
    }
}



