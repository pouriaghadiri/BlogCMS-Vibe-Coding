using AutoMapper;
using BlogCMS.Application.Posts.Commands.UpdatePost;
using BlogCMS.Domain.Entities;
using BlogCMS.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace BlogCMS.Tests.Posts;

public class UpdatePostCommandTests
{
    private readonly Mock<IRepository<Post>> _postRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UpdatePostCommandHandler _handler;

    public UpdatePostCommandTests()
    {
        _postRepositoryMock = new Mock<IRepository<Post>>();
        _mapperMock = new Mock<IMapper>();
        _handler = new UpdatePostCommandHandler(_postRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ValidUpdate_ShouldUpdatePost()
    {
        // Arrange
        var postId = Guid.NewGuid();
        var command = new UpdatePostCommand
        {
            Id = postId,
            Title = "Updated Title",
            Content = "Updated Content",
            Summary = "Updated Summary",
            CategoryId = Guid.NewGuid()
        };

        var existingPost = new Post
        {
            Id = postId,
            Title = "Original Title",
            Content = "Original Content",
            Summary = "Original Summary",
            CategoryId = Guid.NewGuid()
        };

        _postRepositoryMock.Setup(x => x.GetByIdAsync(postId))
            .ReturnsAsync(existingPost);

        _postRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Post>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(postId);
        _postRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Post>()), Times.Once);
    }

    [Fact]
    public async Task Handle_PostNotFound_ShouldThrowException()
    {
        // Arrange
        var postId = Guid.NewGuid();
        var command = new UpdatePostCommand
        {
            Id = postId,
            Title = "Updated Title",
            Content = "Updated Content",
            Summary = "Updated Summary",
            CategoryId = Guid.NewGuid()
        };

        _postRepositoryMock.Setup(x => x.GetByIdAsync(postId))
            .ReturnsAsync((Post)null);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => 
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_UpdateFails_ShouldThrowException()
    {
        // Arrange
        var postId = Guid.NewGuid();
        var command = new UpdatePostCommand
        {
            Id = postId,
            Title = "Updated Title",
            Content = "Updated Content",
            Summary = "Updated Summary",
            CategoryId = Guid.NewGuid()
        };

        var existingPost = new Post
        {
            Id = postId,
            Title = "Original Title",
            Content = "Original Content",
            Summary = "Original Summary",
            CategoryId = Guid.NewGuid()
        };

        _postRepositoryMock.Setup(x => x.GetByIdAsync(postId))
            .ReturnsAsync(existingPost);

        _postRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Post>()))
            .ThrowsAsync(new Exception("Update failed"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => 
            _handler.Handle(command, CancellationToken.None));
    }
} 