using System.Threading.Tasks;

namespace Bil372Project.BusinessLayer.Services;

public interface IAuditLogService
{
    Task LogAsync(int? userId, string tableName, string action, object? oldValues = null, object? newValues = null);
}