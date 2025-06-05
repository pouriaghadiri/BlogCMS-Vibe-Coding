using BlogCMS.Application.Users.DTOs;
using MediatR;

namespace BlogCMS.Application.Users.Commands.UpdateCurrentUser;

public record UpdateCurrentUserCommand(UpdateUserDto User) : IRequest<UserDto>; 