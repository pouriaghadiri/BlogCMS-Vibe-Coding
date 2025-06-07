using FluentValidation;

namespace BlogCMS.Application.Users.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Login.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("A valid email address is required.");

        RuleFor(x => x.Login.Password)
            .NotEmpty()
            .WithMessage("Password is required.");
    }
} 