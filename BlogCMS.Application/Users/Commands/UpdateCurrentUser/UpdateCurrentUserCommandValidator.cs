using FluentValidation;

namespace BlogCMS.Application.Users.Commands.UpdateCurrentUser;

public class UpdateCurrentUserCommandValidator : AbstractValidator<UpdateCurrentUserCommand>
{
    public UpdateCurrentUserCommandValidator()
    {
        RuleFor(x => x.User.FirstName)
            .MaximumLength(50)
            .When(x => !string.IsNullOrEmpty(x.User.FirstName))
            .WithMessage("First name must not exceed 50 characters.");

        RuleFor(x => x.User.LastName)
            .MaximumLength(50)
            .When(x => !string.IsNullOrEmpty(x.User.LastName))
            .WithMessage("Last name must not exceed 50 characters.");

        RuleFor(x => x.User.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrEmpty(x.User.Email))
            .WithMessage("A valid email address is required.");

        RuleFor(x => x.User.PhoneNumber)
            .MaximumLength(20)
            .When(x => !string.IsNullOrEmpty(x.User.PhoneNumber))
            .WithMessage("Phone number must not exceed 20 characters.");

        RuleForEach(x => x.User.Roles)
            .NotEmpty()
            .MaximumLength(50)
            .When(x => x.User.Roles != null && x.User.Roles.Any())
            .WithMessage("Each role must not be empty and must not exceed 50 characters.");
    }
} 