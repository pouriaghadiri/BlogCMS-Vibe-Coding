using AutoMapper;
using BlogCMS.Application.Users.Commands.Login;
using BlogCMS.Application.Users.DTOs;
using BlogCMS.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace BlogCMS.Tests.Users;

public class LoginCommandTests
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly LoginCommandHandler _handler;

    public LoginCommandTests()
    {
        var userStoreMock = new Mock<IUserStore<User>>();
        _userManagerMock = new Mock<UserManager<User>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);
        _mapperMock = new Mock<IMapper>();
        _configurationMock = new Mock<IConfiguration>();

        // Setup JWT configuration with a key that's at least 32 bytes (256 bits) long
        _configurationMock.Setup(x => x["Jwt:Key"]).Returns("your-256-bit-secret-key-here-12345678901234567890123456789012");
        _configurationMock.Setup(x => x["Jwt:Issuer"]).Returns("test-issuer");
        _configurationMock.Setup(x => x["Jwt:Audience"]).Returns("test-audience");
        _configurationMock.Setup(x => x["Jwt:ExpiryInMinutes"]).Returns("60");

        _handler = new LoginCommandHandler(
            _userManagerMock.Object,
            _mapperMock.Object,
            _configurationMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ShouldReturnLoginResponse()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "test@example.com",
            Password = "Test123!"
        };

        var command = new LoginCommand(loginDto);

        var user = new User
        {
            Id = "1",
            UserName = "testuser",
            Email = loginDto.Email,
            FirstName = "Test",
            LastName = "User"
        };

        var userDto = new UserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Roles = new List<string> { "User" }
        };

        _userManagerMock.Setup(x => x.FindByEmailAsync(loginDto.Email))
            .ReturnsAsync(user);

        _userManagerMock.Setup(x => x.CheckPasswordAsync(user, loginDto.Password))
            .ReturnsAsync(true);

        _userManagerMock.Setup(x => x.GetRolesAsync(user))
            .ReturnsAsync(new List<string> { "User" });

        _mapperMock.Setup(x => x.Map<UserDto>(user))
            .Returns(userDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
        result.User.Should().BeEquivalentTo(userDto);

        _userManagerMock.Verify(x => x.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidEmail_ShouldThrowException()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "nonexistent@example.com",
            Password = "Test123!"
        };

        var command = new LoginCommand(loginDto);

        _userManagerMock.Setup(x => x.FindByEmailAsync(loginDto.Email))
            .ReturnsAsync((User)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_InvalidPassword_ShouldThrowException()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "test@example.com",
            Password = "WrongPassword123!"
        };

        var command = new LoginCommand(loginDto);

        var user = new User
        {
            Id = "1",
            UserName = "testuser",
            Email = loginDto.Email
        };

        _userManagerMock.Setup(x => x.FindByEmailAsync(loginDto.Email))
            .ReturnsAsync(user);

        _userManagerMock.Setup(x => x.CheckPasswordAsync(user, loginDto.Password))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _handler.Handle(command, CancellationToken.None));
    }
} 