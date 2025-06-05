using BlogCMS.Domain.Common;

namespace BlogCMS.Domain.Entities;

public class Comment : BaseEntity
{
    public string Content { get; set; } = string.Empty;
    public bool IsApproved { get; set; }
    public Guid? ParentId { get; set; }

    // Foreign keys
    public Guid PostId { get; set; }
    public string UserId { get; set; } = string.Empty;

    // Navigation properties
    public Post Post { get; set; } = null!;
    public User User { get; set; } = null!;
    public Comment? Parent { get; set; }
    public ICollection<Comment> Replies { get; set; } = new List<Comment>();
} 