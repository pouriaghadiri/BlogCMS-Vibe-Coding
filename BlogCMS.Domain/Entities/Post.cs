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
    public string AuthorId { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }

    // Navigation properties
    public User Author { get; set; } = null!;
    public Category Category { get; set; } = null!;
    public ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}

/// <summary>
/// Represents the current state of a post in the system.
/// </summary>
public enum PostStatus
{
    /// <summary>
    /// Post is in draft state and not visible to the public.
    /// Can be edited and published later.
    /// </summary>
    Draft,

    /// <summary>
    /// Post is published and visible to the public.
    /// Can be edited by authorized users.
    /// </summary>
    Published,

    /// <summary>
    /// Post is scheduled to be published at a specific time.
    /// Will automatically change to Published when the scheduled time is reached.
    /// </summary>
    Scheduled,

    /// <summary>
    /// Post was previously published but has been unpublished.
    /// Not visible to the public but can be republished.
    /// </summary>
    Unpublished,

    /// <summary>
    /// Post is archived and cannot be modified.
    /// Archived posts are still visible but cannot be edited by anyone.
    /// </summary>
    Archived
} 