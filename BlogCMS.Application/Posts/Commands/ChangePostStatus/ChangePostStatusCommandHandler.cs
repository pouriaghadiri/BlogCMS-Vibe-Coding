using BlogCMS.Domain.Entities;
using BlogCMS.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BlogCMS.Application.Posts.Commands.ChangePostStatus;

public class ChangePostStatusCommandHandler : IRequestHandler<ChangePostStatusCommand, bool>
{
    private readonly IRepository<Post> _postRepository;
    private readonly UserManager<User> _userManager;
    private readonly IUnitOfWork _unitOfWork;

    public ChangePostStatusCommandHandler(
        IRepository<Post> postRepository,
        UserManager<User> userManager,
        IUnitOfWork unitOfWork)
    {
        _postRepository = postRepository;
        _userManager = userManager;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(ChangePostStatusCommand request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetByIdAsync(request.PostId);
        if (post == null)
        {
            throw new KeyNotFoundException($"Post with ID {request.PostId} not found.");
        }

        // Parse the new status
        if (!Enum.TryParse<PostStatus>(request.NewStatus, out var newStatus))
        {
            throw new ArgumentException($"Invalid status: {request.NewStatus}");
        }

        // Validate status transition
        ValidateStatusTransition(post.Status, newStatus);

        // Update post status
        post.Status = newStatus;
        
        // Handle scheduled posts
        if (newStatus == PostStatus.Scheduled)
        {
            if (!request.PublishDate.HasValue)
            {
                throw new ArgumentException("Publish date is required for scheduled posts.");
            }
            post.PublishedAt = request.PublishDate.Value;
        }
        else if (newStatus == PostStatus.Published)
        {
            post.PublishedAt = DateTime.UtcNow;
        }
        else if (newStatus == PostStatus.Unpublished)
        {
            // Keep the PublishedAt date for history
            // This allows us to know when the post was last published
        }

        await _postRepository.UpdateAsync(post);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    private void ValidateStatusTransition(PostStatus currentStatus, PostStatus newStatus)
    {
        // Archived posts cannot be modified
        if (currentStatus == PostStatus.Archived)
        {
            throw new InvalidOperationException("Archived posts cannot be modified.");
        }

        // Cannot archive a draft or unpublished post
        if ((currentStatus == PostStatus.Draft || currentStatus == PostStatus.Unpublished) 
            && newStatus == PostStatus.Archived)
        {
            throw new InvalidOperationException("Only published posts can be archived.");
        }

        // Validate other transitions
        switch (currentStatus)
        {
            case PostStatus.Draft:
                // Draft can transition to Published, Scheduled, or Unpublished
                if (newStatus != PostStatus.Published && 
                    newStatus != PostStatus.Scheduled && 
                    newStatus != PostStatus.Unpublished)
                {
                    throw new InvalidOperationException("Draft posts can only be changed to Published, Scheduled, or Unpublished.");
                }
                break;

            case PostStatus.Published:
                // Published can transition to Unpublished or Archived
                if (newStatus != PostStatus.Unpublished && newStatus != PostStatus.Archived)
                {
                    throw new InvalidOperationException("Published posts can only be changed to Unpublished or Archived.");
                }
                break;

            case PostStatus.Scheduled:
                // Scheduled can transition to Draft, Published, or Unpublished
                if (newStatus != PostStatus.Draft && 
                    newStatus != PostStatus.Published && 
                    newStatus != PostStatus.Unpublished)
                {
                    throw new InvalidOperationException("Scheduled posts can only be changed to Draft, Published, or Unpublished.");
                }
                break;

            case PostStatus.Unpublished:
                // Unpublished can transition to Draft, Published, or Scheduled
                if (newStatus != PostStatus.Draft && 
                    newStatus != PostStatus.Published && 
                    newStatus != PostStatus.Scheduled)
                {
                    throw new InvalidOperationException("Unpublished posts can only be changed to Draft, Published, or Scheduled.");
                }
                break;
        }
    }
} 