using SmartAuditDemo.Domain.Enums;

namespace SmartAuditDemo.Application.DTOs;

public class AccountingDocumentFilterDto
{
    public string? Title { get; set; }
    public DocumentType? DocumentType { get; set; }
    public string? Extension { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

