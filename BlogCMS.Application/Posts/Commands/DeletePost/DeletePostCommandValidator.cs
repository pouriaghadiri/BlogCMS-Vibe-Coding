using FluentValidation;

namespace BlogCMS.Application.Posts.Commands.DeletePost;

public class DeletePostCommandValidator : AbstractValidator<DeletePostCommand>
{
    public DeletePostCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Post ID must not be empty.");
    }
} 