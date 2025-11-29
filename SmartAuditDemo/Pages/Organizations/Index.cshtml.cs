using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartAuditDemo.Application.DTOs;
using SmartAuditDemo.Domain.Enums;
using SmartAuditDemo.Services;

namespace SmartAuditDemo.Pages.Organizations;

public class IndexModel : PageModel
{
    private readonly ApiService _apiService;

    public IndexModel(ApiService apiService)
    {
        _apiService = apiService;
    }

    public PagedResultDto<OrganizationDto>? Organizations { get; set; }
    public OrganizationFilterDto Filter { get; set; } = new();

    public async Task OnGetAsync(string? name, string? economicCode, int? companyType, bool? isActive, int pageNumber = 1)
    {
        Filter.Name = name;
        Filter.EconomicCode = economicCode;
        Filter.CompanyType = companyType.HasValue ? (CompanyType)companyType.Value : null;
        Filter.IsActive = isActive;
        Filter.PageNumber = pageNumber;
        Filter.PageSize = 10;

        Organizations = await _apiService.GetOrganizationsAsync(Filter);
    }
}



