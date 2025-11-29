using SmartAuditDemo.Domain.Enums;

namespace SmartAuditDemo.Application.DTOs;

public class AccountingDocumentDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DocumentType DocumentType { get; set; }
    public string DocumentTypeName { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public Guid UploaderUserId { get; set; }
    public string UploaderUserName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public long SizeInKb { get; set; }
    public string SizeFormatted { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }
}
