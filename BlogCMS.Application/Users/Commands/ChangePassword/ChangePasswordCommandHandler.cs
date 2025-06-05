using BlogCMS.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace BlogCMS.Application.Users.Commands.ChangePassword;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, bool>
{
    private readonly UserManager<User> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ChangePasswordCommandHandler(
        UserManager<User> userManager,
        IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        if (_httpContextAccessor.HttpContext?.User == null)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
        if (user == null)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        var result = await _userManager.ChangePasswordAsync(
            user,
            request.ChangePassword.CurrentPassword,
            request.ChangePassword.NewPassword);

        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Failed to change password: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        return true;
    }
} 