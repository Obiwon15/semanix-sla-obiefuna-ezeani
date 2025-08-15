using Microsoft.EntityFrameworkCore;
using FormsService.Domain.Aggregates;
using FormsService.Infrastructure.Persistence.Configurations;
using System.Text.Json;

namespace FormsService.Infrastructure.Persistence;

public class FormsDbContext : DbContext
{
    public FormsDbContext(DbContextOptions<FormsDbContext> options) : base(options) { }

    public DbSet<Form> Forms { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new FormConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}