using SmartAuditDemo.Domain.Enums;

namespace SmartAuditDemo.Domain.Entities;

public class Organization
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NationalId { get; set; } = string.Empty;
    public string RegistrationNumber { get; set; } = string.Empty;
    public string EconomicCode { get; set; } = string.Empty;
    public CompanyType CompanyType { get; set; }
    public BusinessField BusinessField { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new ArgumentException("نام شرکت الزامی است", nameof(Name));

        if (string.IsNullOrWhiteSpace(NationalId))
            throw new ArgumentException("شناسه ملی الزامی است", nameof(NationalId));

        if (NationalId.Length < 10 || NationalId.Length > 11)
            throw new ArgumentException("شناسه ملی باید 10 یا 11 کاراکتر باشد", nameof(NationalId));

        if (string.IsNullOrWhiteSpace(RegistrationNumber))
            throw new ArgumentException("شماره ثبت الزامی است", nameof(RegistrationNumber));

        if (string.IsNullOrWhiteSpace(EconomicCode))
            throw new ArgumentException("کد اقتصادی الزامی است", nameof(EconomicCode));

        if (EconomicCode.Length < 10 || EconomicCode.Length > 12)
            throw new ArgumentException("کد اقتصادی باید بین 10 تا 12 کاراکتر باشد", nameof(EconomicCode));
    }
}

