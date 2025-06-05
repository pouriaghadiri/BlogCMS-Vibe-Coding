using BlogCMS.Application.Users.DTOs;
using MediatR;

namespace BlogCMS.Application.Users.Commands.Login;

public record LoginCommand(LoginDto Login) : IRequest<LoginResponseDto>; 