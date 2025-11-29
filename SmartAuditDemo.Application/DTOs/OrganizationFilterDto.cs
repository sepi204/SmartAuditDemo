using SmartAuditDemo.Domain.Enums;

namespace SmartAuditDemo.Application.DTOs;

public class OrganizationFilterDto
{
    public string? Name { get; set; }
    public string? EconomicCode { get; set; }
    public CompanyType? CompanyType { get; set; }
    public bool? IsActive { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

