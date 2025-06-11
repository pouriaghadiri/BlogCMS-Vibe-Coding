using MediatR;

namespace BlogCMS.Application.Posts.Commands.ChangePostStatus;

public record ChangePostStatusCommand : IRequest<bool>
{
    public Guid PostId { get; init; }
    public string NewStatus { get; init; } = string.Empty;
    public DateTime? PublishDate { get; init; }
} 