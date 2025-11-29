using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using SmartAuditDemo.Domain.Enums;

namespace SmartAuditDemo.Application.DTOs;

public class CreateAccountingDocumentDto
{
    [Required(ErrorMessage = "عنوان سند الزامی است")]
    [StringLength(500, ErrorMessage = "عنوان سند نمی‌تواند بیشتر از 500 کاراکتر باشد")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "نوع سند الزامی است")]
    public DocumentType DocumentType { get; set; }

    [Required(ErrorMessage = "فایل الزامی است")]
    public IFormFile File { get; set; } = null!;

    public Guid UploaderUserId { get; set; }
}
