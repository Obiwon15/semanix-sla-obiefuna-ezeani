using Microsoft.EntityFrameworkCore;
using RenderingService.Domain.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace RenderingService.Infrastructure.Persistence;

public class RenderingDbContext : DbContext
{
    public RenderingDbContext(DbContextOptions<RenderingDbContext> options) : base(options) { }
    public DbSet<RenderedForm> RenderedForms { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RenderedForm>(entity =>
        {
            entity.ToTable("RenderedForms");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.TenantId)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.EntityId)
                .HasMaxLength(100);

            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.Description)
                .HasMaxLength(1000);

            entity.Property(e => e.FormDefinition)
                .HasColumnType("nvarchar(max)")
                .IsRequired();

            entity.HasIndex(e => new { e.TenantId, e.Id });
            entity.HasIndex(e => new { e.TenantId, e.EntityId });
        });

        base.OnModelCreating(modelBuilder);
    }
}