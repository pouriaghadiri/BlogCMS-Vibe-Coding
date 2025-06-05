using BlogCMS.Domain.Common;

namespace BlogCMS.Domain.Entities;

public class Post : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public PostStatus Status { get; set; }
    public DateTime? PublishedAt { get; set; }
    public string? FeaturedImageUrl { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }

    // Foreign keys
    public Guid AuthorId { get; set; }
    public Guid CategoryId { get; set; }

    // Navigation properties
    public User Author { get; set; } = null!;
    public Category Category { get; set; } = null!;
    public ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}

public enum PostStatus
{
    Draft,
    Published,
    Scheduled
} 