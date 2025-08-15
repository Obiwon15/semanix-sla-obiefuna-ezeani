using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FormsService.Domain.Aggregates;
using FormsService.Domain.ValueObjects;
using System.Text.Json;

namespace FormsService.Infrastructure.Persistence.Configurations;

public class FormConfiguration : IEntityTypeConfiguration<Form>
{
    public void Configure(EntityTypeBuilder<Form> builder)
    {
        builder.ToTable("Forms");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.Id)
            .HasConversion(
                id => id.Value,
                value => new FormId(value))
            .ValueGeneratedNever();

        builder.Property(f => f.TenantId)
            .HasConversion(
                tenantId => tenantId.Value,
                value => new TenantId(value))
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(f => f.EntityId)
            .HasConversion(
                entityId => entityId != null ? entityId.Value : null,
                value => value != null ? new EntityId(value) : null)
            .HasMaxLength(100);

        builder.Property(f => f.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(f => f.Description)
            .HasMaxLength(1000);

        builder.Property(f => f.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(f => f.Sections)
            .HasConversion(
                sections => JsonSerializer.Serialize(sections, (JsonSerializerOptions)null!),
                json => JsonSerializer.Deserialize<List<Section>>(json, (JsonSerializerOptions)null!) ?? new List<Section>())
            .HasColumnType("nvarchar(max)");

        builder.Property(f => f.Version)
            .IsRequired();

        builder.Property(f => f.CreatedAt)
            .IsRequired();

        builder.Property(f => f.UpdatedAt)
            .IsRequired();

        builder.HasIndex(f => new { f.TenantId, f.Id });
        builder.HasIndex(f => new { f.TenantId, f.EntityId });
        builder.HasIndex(f => new { f.TenantId, f.Status });

        // Ignore domain events for persistence
        builder.Ignore(f => f.DomainEvents);
    }
}
