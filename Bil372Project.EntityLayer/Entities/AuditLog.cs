using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bil372Project.EntityLayer.Entities;

public class AuditLog
{
    [Key]
    public Guid Id { get; set; }

    public int? UserId { get; set; }

    [MaxLength(200)]
    public string TableName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Action { get; set; } = string.Empty;

    [Column(TypeName = "longtext")]
    public string? OldValues { get; set; }

    [Column(TypeName = "longtext")]
    public string? NewValues { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey(nameof(UserId))]
    public AppUser? User { get; set; }
}