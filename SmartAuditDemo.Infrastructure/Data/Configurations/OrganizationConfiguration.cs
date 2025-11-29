using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartAuditDemo.Domain.Entities;

namespace SmartAuditDemo.Infrastructure.Data.Configurations;

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ToTable("Organization");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .IsRequired();

        builder.Property(o => o.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(o => o.NationalId)
            .IsRequired()
            .HasMaxLength(11);

        builder.HasIndex(o => o.NationalId)
            .IsUnique();

        builder.Property(o => o.RegistrationNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(o => o.EconomicCode)
            .IsRequired()
            .HasMaxLength(12);

        builder.HasIndex(o => o.EconomicCode)
            .IsUnique();

        builder.Property(o => o.CompanyType)
            .IsRequired();

        builder.Property(o => o.BusinessField)
            .IsRequired();

        builder.Property(o => o.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(o => o.CreatedAt)
            .IsRequired();

        builder.Property(o => o.UpdatedAt)
            .IsRequired(false);
    }
}

