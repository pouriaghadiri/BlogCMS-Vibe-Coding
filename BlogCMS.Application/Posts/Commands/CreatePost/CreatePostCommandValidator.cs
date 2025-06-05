using BlogCMS.Domain.Entities;
using FluentValidation;

namespace BlogCMS.Application.Posts.Commands.CreatePost;

public class CreatePostCommandValidator : AbstractValidator<CreatePostCommand>
{
    public CreatePostCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200)
            .WithMessage("Title must not be empty and must not exceed 200 characters.");

        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("Content must not be empty.");

        RuleFor(x => x.Summary)
            .NotEmpty()
            .MaximumLength(500)
            .WithMessage("Summary must not be empty and must not exceed 500 characters.");

        RuleFor(x => x.Status)
            .NotEmpty()
            .Must(status => Enum.TryParse<PostStatus>(status, out _))
            .WithMessage("Status must be a valid post status (Draft, Published, or Scheduled).");

        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .WithMessage("Category ID must not be empty.");

        RuleFor(x => x.Tags)
            .NotNull()
            .WithMessage("Tags must not be null.");

        RuleForEach(x => x.Tags)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("Each tag must not be empty and must not exceed 50 characters.");

        When(x => x.PublishedAt.HasValue, () =>
        {
            RuleFor(x => x.PublishedAt)
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("Published date must be in the future.");
        });

        RuleFor(x => x.MetaTitle)
            .MaximumLength(200)
            .When(x => x.MetaTitle != null)
            .WithMessage("Meta title must not exceed 200 characters.");

        RuleFor(x => x.MetaDescription)
            .MaximumLength(500)
            .When(x => x.MetaDescription != null)
            .WithMessage("Meta description must not exceed 500 characters.");
    }
} 