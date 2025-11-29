using System.ComponentModel.DataAnnotations;
using SmartAuditDemo.Domain.Enums;

namespace SmartAuditDemo.Application.DTOs;

public class UpdateOrganizationDto
{
    [Required(ErrorMessage = "شناسه الزامی است")]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "نام شرکت الزامی است")]
    [StringLength(200, ErrorMessage = "نام شرکت نمی‌تواند بیشتر از 200 کاراکتر باشد")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "شناسه ملی الزامی است")]
    [StringLength(11, MinimumLength = 10, ErrorMessage = "شناسه ملی باید 10 یا 11 کاراکتر باشد")]
    public string NationalId { get; set; } = string.Empty;

    [Required(ErrorMessage = "شماره ثبت الزامی است")]
    [StringLength(50, ErrorMessage = "شماره ثبت نمی‌تواند بیشتر از 50 کاراکتر باشد")]
    public string RegistrationNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "کد اقتصادی الزامی است")]
    [StringLength(12, MinimumLength = 10, ErrorMessage = "کد اقتصادی باید بین 10 تا 12 کاراکتر باشد")]
    public string EconomicCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "نوع شرکت الزامی است")]
    public CompanyType CompanyType { get; set; }

    [Required(ErrorMessage = "زمینه فعالیت الزامی است")]
    public BusinessField BusinessField { get; set; }

    public bool IsActive { get; set; }
}

