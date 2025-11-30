using System.Text.Json;
using Bil372Project.DataAccessLayer;
using Bil372Project.EntityLayer.Entities;

namespace Bil372Project.BusinessLayer.Services;

public class AuditLogService : IAuditLogService
{
    private readonly AppDbContext _context;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    public AuditLogService(AppDbContext context)
    {
        _context = context;
    }

    public async Task LogAsync(int? userId, string tableName, string action, object? oldValues = null, object? newValues = null)
    {
        var audit = new AuditLog
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TableName = tableName,
            Action = action,
            OldValues = oldValues == null ? null : JsonSerializer.Serialize(oldValues, JsonOptions),
            NewValues = newValues == null ? null : JsonSerializer.Serialize(newValues, JsonOptions),
            CreatedAt = DateTime.UtcNow
        };

        _context.AuditLogs.Add(audit);
        await _context.SaveChangesAsync();
    }
}