using FluentValidation;

namespace BlogCMS.Application.Posts.Commands.UpdatePost;

public class UpdatePostCommandValidator : AbstractValidator<UpdatePostCommand>
{
    public UpdatePostCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Post ID must not be empty.");

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

        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .WithMessage("Category ID must not be empty.");
    }
} 