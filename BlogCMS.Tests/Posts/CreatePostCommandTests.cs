using AutoMapper;
using BlogCMS.Application.Posts.Commands.CreatePost;
using BlogCMS.Domain.Entities;
using BlogCMS.Domain.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Linq.Expressions;
using System.Security.Claims;
using Xunit;

namespace BlogCMS.Tests.Posts;

public class CreatePostCommandTests
{
    private readonly Mock<IRepository<Post>> _postRepositoryMock;
    private readonly Mock<IRepository<Category>> _categoryRepositoryMock;
    private readonly Mock<IRepository<Tag>> _tagRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly CreatePostCommandHandler _handler;

    public CreatePostCommandTests()
    {
        var userStoreMock = new Mock<IUserStore<User>>();
        _postRepositoryMock = new Mock<IRepository<Post>>();
        _categoryRepositoryMock = new Mock<IRepository<Category>>();
        _tagRepositoryMock = new Mock<IRepository<Tag>>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _userManagerMock = new Mock<UserManager<User>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        _handler = new CreatePostCommandHandler(
            _postRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _tagRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _userManagerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidPost_ShouldCreatePost()
    {
        // Arrange
        var userId = "1";
        var categoryId = Guid.NewGuid();
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
        };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };

        _httpContextAccessorMock.Setup(x => x.HttpContext)
            .Returns(httpContext);

        var command = new CreatePostCommand
        {
            Title = "Test Post",
            Content = "Test Content",
            Summary = "Test Summary",
            Status = "Draft",
            CategoryId = categoryId,
            Tags = new List<string> { "test", "blog" },
            User = principal
        };

        var user = new User
        {
            Id = userId,
            UserName = "testuser",
            Email = "test@example.com"
        };

        var category = new Category
        {
            Id = categoryId,
            Name = "Test Category"
        };

        var existingTag = new Tag
        {
            Id = Guid.NewGuid(),
            Name = "test",
            Slug = "test"
        };

        _userManagerMock.Setup(x => x.GetUserAsync(principal))
            .ReturnsAsync(user);

        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(categoryId))
            .ReturnsAsync(category);

        _tagRepositoryMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Tag, bool>>>()))
            .ReturnsAsync(new List<Tag> { existingTag });

        var postId = Guid.NewGuid();
        _postRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Post>()))
            .Callback<Post>(post => post.Id = postId)
            .ReturnsAsync((Post post) => post);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(postId);
        _postRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Post>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_NoHttpContext_ShouldThrowException()
    {
        // Arrange
        _httpContextAccessorMock.Setup(x => x.HttpContext)
            .Returns((HttpContext)null);

        var command = new CreatePostCommand
        {
            Title = "Test Post",
            Content = "Test Content",
            Summary = "Test Summary",
            Status = "Draft",
            CategoryId = Guid.NewGuid(),
            Tags = new List<string>(),
            User = new ClaimsPrincipal()
        };

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


        var command = new CreatePostCommand
        {
            Title = "Test Post",
            Content = "Test Content",
            Summary = "Test Summary",
            Status = "Draft",
            CategoryId = Guid.NewGuid(),
            Tags = new List<string>(),
            User = new ClaimsPrincipal()
        };
        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => 
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_InvalidCategory_ShouldThrowException()
    {
        // Arrange
        var userId = "1";
        var categoryId = Guid.NewGuid();
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

        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(categoryId))
            .ReturnsAsync((Category)null);

        var command = new CreatePostCommand
        {
            CategoryId = categoryId,
            User = principal
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_NewTag_ShouldCreateTag()
    {
        // Arrange
        var userId = "1";
        var categoryId = Guid.NewGuid();
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
        };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };

        _httpContextAccessorMock.Setup(x => x.HttpContext)
            .Returns(httpContext);

        var command = new CreatePostCommand
        {
            Title = "Test Post",
            Content = "Test Content",
            Summary = "Test Summary",
            Status = "Draft",
            CategoryId = categoryId,
            Tags = new List<string> { "newtag" },
            User = principal
        };

        var user = new User
        {
            Id = userId,
            UserName = "testuser",
            Email = "test@example.com"
        };

        var category = new Category
        {
            Id = categoryId,
            Name = "Test Category"
        };

        _userManagerMock.Setup(x => x.GetUserAsync(principal))
            .ReturnsAsync(user);

        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(categoryId))
            .ReturnsAsync(category);

        _tagRepositoryMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Tag, bool>>>()))
            .ReturnsAsync(new List<Tag>());

        _tagRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Tag>()))
            .ReturnsAsync(It.IsAny<Tag>());

        var postId = Guid.NewGuid();
        _postRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Post>()))
            .Callback<Post>(post => post.Id = postId)
            .ReturnsAsync((Post post) => post);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(postId);
        _tagRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Tag>()), Times.Once);
        _postRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Post>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }
} 