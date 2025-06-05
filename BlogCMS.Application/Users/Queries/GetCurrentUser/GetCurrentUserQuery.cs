using BlogCMS.Application.Users.DTOs;
using MediatR;

namespace BlogCMS.Application.Users.Queries.GetCurrentUser;

public record GetCurrentUserQuery : IRequest<UserDto>; 