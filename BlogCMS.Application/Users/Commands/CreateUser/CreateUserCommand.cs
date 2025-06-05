using BlogCMS.Application.Users.DTOs;
using MediatR;

namespace BlogCMS.Application.Users.Commands.CreateUser;

public record CreateUserCommand : IRequest<UserDto>
{
    // Required parameters
    public string UserName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;

    // Optional parameters
    public string? PhoneNumber { get; init; }
    public List<string> Roles { get; init; } = new();
} 