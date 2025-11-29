using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartAuditDemo.Application.DTOs;
using SmartAuditDemo.Domain.Enums;
using SmartAuditDemo.Services;

namespace SmartAuditDemo.Pages.Documents;

public class IndexModel : PageModel
{
    private readonly ApiService _apiService;

    public IndexModel(ApiService apiService)
    {
        _apiService = apiService;
    }

    public PagedResultDto<AccountingDocumentDto>? Documents { get; set; }
    public Dictionary<DocumentType, List<AccountingDocumentDto>>? DocumentsByType { get; set; }
    public AccountingDocumentFilterDto Filter { get; set; } = new();

    public async Task OnGetAsync(string? title, int? documentType, string? extension, int pageNumber = 1)
    {
        Filter.Title = title;
        Filter.DocumentType = documentType.HasValue ? (DocumentType)documentType.Value : null;
        Filter.Extension = extension;
        Filter.PageNumber = pageNumber;
        Filter.PageSize = 50; // بیشتر برای نمایش همه در دسته‌بندی

        Documents = await _apiService.GetDocumentsAsync(Filter);

        if (Documents?.Data != null)
        {
            DocumentsByType = Documents.Data
                .GroupBy(d => d.DocumentType)
                .ToDictionary(g => g.Key, g => g.ToList());
        }
    }
}
