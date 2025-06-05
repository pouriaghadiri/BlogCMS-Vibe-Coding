using BlogCMS.Application.Users.Commands.ChangePassword;
using BlogCMS.Application.Users.DTOs;
using BlogCMS.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Security.Claims;
using Xunit;

namespace BlogCMS.Tests.Users;

public class ChangePasswordCommandTests
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly ChangePasswordCommandHandler _handler;

    public ChangePasswordCommandTests()
    {
        var userStoreMock = new Mock<IUserStore<User>>();
        _userManagerMock = new Mock<UserManager<User>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        _handler = new ChangePasswordCommandHandler(
            _userManagerMock.Object,
            _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task Handle_ValidPasswordChange_ShouldReturnTrue()
    {
        // Arrange
        var userId = "1";
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
        };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };

        _httpContextAccessorMock.Setup(x => x.HttpContext)
            .Returns(httpContext);

        var changePasswordDto = new ChangePasswordDto
        {
            CurrentPassword = "CurrentPass123!",
            NewPassword = "NewPass123!"
        };

        var command = new ChangePasswordCommand(changePasswordDto);

        var user = new User
        {
            Id = userId,
            UserName = "testuser",
            Email = "test@example.com"
        };

        _userManagerMock.Setup(x => x.GetUserAsync(principal))
            .ReturnsAsync(user);

        _userManagerMock.Setup(x => x.ChangePasswordAsync(
                user,
                changePasswordDto.CurrentPassword,
                changePasswordDto.NewPassword))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        _userManagerMock.Verify(x => x.ChangePasswordAsync(
            user,
            changePasswordDto.CurrentPassword,
            changePasswordDto.NewPassword), Times.Once);
    }

    [Fact]
    public async Task Handle_NoHttpContext_ShouldThrowException()
    {
        // Arrange
        _httpContextAccessorMock.Setup(x => x.HttpContext)
            .Returns((HttpContext)null);

        var command = new ChangePasswordCommand(new ChangePasswordDto());

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => 
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_NoUser_ShouldThrowException()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1")
        };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };

        _httpContextAccessorMock.Setup(x => x.HttpContext)
            .Returns(httpContext);

        _userManagerMock.Setup(x => x.GetUserAsync(principal))
            .ReturnsAsync((User)null);

        var command = new ChangePasswordCommand(new ChangePasswordDto());

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => 
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_PasswordChangeFails_ShouldThrowException()
    {
        // Arrange
        var userId = "1";
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
        };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };

        _httpContextAccessorMock.Setup(x => x.HttpContext)
            .Returns(httpContext);

        var user = new User
        {
            Id = userId,
            UserName = "testuser",
            Email = "test@example.com"
        };

        _userManagerMock.Setup(x => x.GetUserAsync(principal))
            .ReturnsAsync(user);

        var errors = new List<IdentityError>
        {
            new() { Code = "PasswordMismatch", Description = "Current password is incorrect" }
        };

        _userManagerMock.Setup(x => x.ChangePasswordAsync(
                It.IsAny<User>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(errors.ToArray()));

        var command = new ChangePasswordCommand(new ChangePasswordDto
        {
            CurrentPassword = "WrongPass123!",
            NewPassword = "NewPass123!"
        });

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _handler.Handle(command, CancellationToken.None));
    }
} 