using AutoMapper;
using BlogCMS.Application.Users.DTOs;
using BlogCMS.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace BlogCMS.Application.Users.Commands.UpdateCurrentUser;

public class UpdateCurrentUserCommandHandler : IRequestHandler<UpdateCurrentUserCommand, UserDto>
{
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UpdateCurrentUserCommandHandler(
        UserManager<User> userManager,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<UserDto> Handle(UpdateCurrentUserCommand request, CancellationToken cancellationToken)
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

        // Update user properties
        if (!string.IsNullOrEmpty(request.User.FirstName))
            user.FirstName = request.User.FirstName;
        if (!string.IsNullOrEmpty(request.User.LastName))
            user.LastName = request.User.LastName;
        if (!string.IsNullOrEmpty(request.User.PhoneNumber))
            user.PhoneNumber = request.User.PhoneNumber;
        if (!string.IsNullOrEmpty(request.User.Email) && request.User.Email != user.Email)
        {
            user.Email = request.User.Email;
            user.EmailConfirmed = false;
        }

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Failed to update user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        var userDto = _mapper.Map<UserDto>(user);
        userDto.Roles = (await _userManager.GetRolesAsync(user)).ToList();

        return userDto;
    }
} 