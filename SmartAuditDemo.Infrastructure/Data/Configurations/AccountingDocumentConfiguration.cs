using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartAuditDemo.Domain.Entities;

namespace SmartAuditDemo.Infrastructure.Data.Configurations;

public class AccountingDocumentConfiguration : IEntityTypeConfiguration<AccountingDocument>
{
    public void Configure(EntityTypeBuilder<AccountingDocument> builder)
    {
        builder.ToTable("AccountingDocument");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
            .IsRequired();

        builder.Property(d => d.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(d => d.DocumentType)
            .IsRequired();

        builder.HasIndex(d => d.DocumentType);

        builder.Property(d => d.Extension)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(d => d.FilePath)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(d => d.UploaderUserId)
            .IsRequired();

        builder.Property(d => d.CreatedAt)
            .IsRequired();

        builder.Property(d => d.SizeInKb)
            .IsRequired();

        builder.Property(d => d.UpdatedAt)
            .IsRequired(false);
    }
}
