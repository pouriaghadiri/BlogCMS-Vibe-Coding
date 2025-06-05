using MediatR;

namespace BlogCMS.Application.Posts.Commands.CreatePost;

public record CreatePostCommand : IRequest<Guid>
{
    public string Title { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public string Summary { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime? PublishedAt { get; init; }
    public string? FeaturedImageUrl { get; init; }
    public string? MetaTitle { get; init; }
    public string? MetaDescription { get; init; }
    public string? MetaKeywords { get; init; }
    public Guid CategoryId { get; init; }
    public List<string> Tags { get; init; } = new();
    public string AuthorId { get; init; } = string.Empty;
} 