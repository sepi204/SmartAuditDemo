using SmartAuditDemo.Domain.Enums;

namespace SmartAuditDemo.Domain.Entities;

public class AccountingDocument
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DocumentType DocumentType { get; set; }
    public string Extension { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public Guid UploaderUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public long SizeInKb { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Title))
            throw new ArgumentException("عنوان سند الزامی است", nameof(Title));

        if (string.IsNullOrWhiteSpace(Extension))
            throw new ArgumentException("نوع فایل الزامی است", nameof(Extension));

        if (string.IsNullOrWhiteSpace(FilePath))
            throw new ArgumentException("مسیر فایل الزامی است", nameof(FilePath));

        if (SizeInKb <= 0)
            throw new ArgumentException("حجم فایل باید بیشتر از صفر باشد", nameof(SizeInKb));
    }
}
