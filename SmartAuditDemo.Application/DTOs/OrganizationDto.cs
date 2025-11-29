using SmartAuditDemo.Domain.Enums;

namespace SmartAuditDemo.Application.DTOs;

public class OrganizationDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NationalId { get; set; } = string.Empty;
    public string RegistrationNumber { get; set; } = string.Empty;
    public string EconomicCode { get; set; } = string.Empty;
    public CompanyType CompanyType { get; set; }
    public string CompanyTypeName { get; set; } = string.Empty;
    public BusinessField BusinessField { get; set; }
    public string BusinessFieldName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}



