using BlogCMS.Application.Posts.Commands.DeletePost;
using BlogCMS.Domain.Entities;
using BlogCMS.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace BlogCMS.Tests.Posts;

public class DeletePostCommandTests
{
    private readonly Mock<IRepository<Post>> _postRepositoryMock;
    private readonly DeletePostCommandHandler _handler;

    public DeletePostCommandTests()
    {
        _postRepositoryMock = new Mock<IRepository<Post>>();
        _handler = new DeletePostCommandHandler(_postRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidDelete_ShouldDeletePost()
    {
        // Arrange
        var postId = Guid.NewGuid();
        var command = new DeletePostCommand { Id = postId };

        var existingPost = new Post
        {
            Id = postId,
            Title = "Test Post",
            Content = "Test Content",
            Summary = "Test Summary"
        };

        _postRepositoryMock.Setup(x => x.GetByIdAsync(postId))
            .ReturnsAsync(existingPost);

        _postRepositoryMock.Setup(x => x.DeleteAsync(existingPost))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        _postRepositoryMock.Verify(x => x.DeleteAsync(existingPost), Times.Once);
    }

    [Fact]
    public async Task Handle_PostNotFound_ShouldThrowException()
    {
        // Arrange
        var postId = Guid.NewGuid();
        var command = new DeletePostCommand { Id = postId };

        _postRepositoryMock.Setup(x => x.GetByIdAsync(postId))
            .ReturnsAsync((Post)null);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => 
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_DeleteFails_ShouldReturnFalse()
    {
        // Arrange
        var postId = Guid.NewGuid();
        var command = new DeletePostCommand { Id = postId };

        var existingPost = new Post
        {
            Id = postId,
            Title = "Test Post",
            Content = "Test Content",
            Summary = "Test Summary"
        };

        _postRepositoryMock.Setup(x => x.GetByIdAsync(postId))
            .ReturnsAsync(existingPost);

        _postRepositoryMock.Setup(x => x.DeleteAsync(existingPost))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
        _postRepositoryMock.Verify(x => x.DeleteAsync(existingPost), Times.Once);
    }
} 