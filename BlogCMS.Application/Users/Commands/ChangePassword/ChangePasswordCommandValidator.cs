using FluentValidation;

namespace BlogCMS.Application.Users.Commands.ChangePassword;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.ChangePassword.CurrentPassword)
            .NotEmpty()
            .WithMessage("Current password is required.");

        RuleFor(x => x.ChangePassword.NewPassword)
            .NotEmpty()
            .MinimumLength(6)
            .MaximumLength(100)
            .Matches("[A-Z]").WithMessage("New password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("New password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("New password must contain at least one number.")
            .Matches("[^a-zA-Z0-9]").WithMessage("New password must contain at least one special character.");

        RuleFor(x => x.ChangePassword.ConfirmPassword)
            .NotEmpty()
            .Equal(x => x.ChangePassword.NewPassword)
            .WithMessage("The new password and confirmation password do not match.");
    }
} 