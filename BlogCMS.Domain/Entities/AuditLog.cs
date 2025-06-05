using BlogCMS.Domain.Common;

namespace BlogCMS.Domain.Entities;

public class AuditLog : BaseEntity
{
    public string Action { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string? DataSnapshot { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }

    // Foreign key
    public string UserId { get; set; } = string.Empty;

    // Navigation property
    public User User { get; set; } = null!;
} 