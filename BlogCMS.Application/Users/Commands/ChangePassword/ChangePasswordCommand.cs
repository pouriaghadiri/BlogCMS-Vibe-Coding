using BlogCMS.Application.Users.DTOs;
using MediatR;

namespace BlogCMS.Application.Users.Commands.ChangePassword;

public record ChangePasswordCommand(ChangePasswordDto ChangePassword) : IRequest<bool>; 