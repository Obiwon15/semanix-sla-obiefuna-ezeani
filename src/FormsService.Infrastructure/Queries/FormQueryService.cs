using Dapper;
using Microsoft.Data.SqlClient;
using FormsService.Application.DTOs;
using FormsService.Application.Services;
using FormsService.Domain.Enums;
using System.Text.Json;

namespace FormsService.Infrastructure.Queries;

public class FormQueryService : IFormQueryService
{
    private readonly string _connectionString;

    public FormQueryService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<FormDto?> GetFormAsync(Guid formId, string tenantId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT Id, TenantId, EntityId, Name, Description, Status, Version, Sections, CreatedAt, UpdatedAt
            FROM Forms 
            WHERE Id = @FormId AND TenantId = @TenantId";

        using var connection = new SqlConnection(_connectionString);
        var result = await connection.QuerySingleOrDefaultAsync<FormQueryResult>(sql, new { FormId = formId, TenantId = tenantId });

        return result?.ToDto();
    }

    public async Task<List<FormDto>> GetFormsAsync(string tenantId, string? entityId = null, CancellationToken cancellationToken = default)
    {
        var sql = @"
            SELECT Id, TenantId, EntityId, Name, Description, Status, Version, Sections, CreatedAt, UpdatedAt
            FROM Forms 
            WHERE TenantId = @TenantId";

        var parameters = new { TenantId = tenantId, EntityId = entityId };

        if (!string.IsNullOrEmpty(entityId))
        {
            sql += " AND EntityId = @EntityId";
        }

        sql += " ORDER BY UpdatedAt DESC";

        using var connection = new SqlConnection(_connectionString);
        var results = await connection.QueryAsync<FormQueryResult>(sql, parameters);

        return results.Select(r => r.ToDto()).ToList();
    }

    private class FormQueryResult
    {
        public Guid Id { get; set; }
        public string TenantId { get; set; } = string.Empty;
        public string? EntityId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Status { get; set; } = string.Empty;
        public int Version { get; set; }
        public string Sections { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public FormDto ToDto()
        {
            var sections = JsonSerializer.Deserialize<List<SectionDto>>(Sections) ?? new List<SectionDto>();
            return new FormDto
            {
                Id = Id,
                TenantId = TenantId,
                EntityId = EntityId,
                Name = Name,
                Description = Description,
                Status = Enum.Parse<FormStatus>(Status),
                Version = Version,
                Sections = sections,
                CreatedAt = CreatedAt,
                UpdatedAt = UpdatedAt
            };
        }
    }
}