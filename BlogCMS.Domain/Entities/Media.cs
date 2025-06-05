using BlogCMS.Domain.Common;

namespace BlogCMS.Domain.Entities;

public class Media : BaseEntity
{
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string? AltText { get; set; }
    public string? Caption { get; set; }

    // Foreign key
    public string UploaderId { get; set; } = string.Empty;

    // Navigation property
    public User Uploader { get; set; } = null!;
} 