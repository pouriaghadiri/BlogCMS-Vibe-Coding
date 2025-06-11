using BlogCMS.Domain.Entities;
using BlogCMS.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BlogCMS.Infrastructure.Services;

public class ScheduledPostService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ScheduledPostService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);

    public ScheduledPostService(
        IServiceProvider serviceProvider,
        ILogger<ScheduledPostService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessScheduledPostsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing scheduled posts");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }
    }

    private async Task ProcessScheduledPostsAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var postRepository = scope.ServiceProvider.GetRequiredService<IRepository<Post>>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var now = DateTime.UtcNow;
        var scheduledPosts = await postRepository.FindAsync(p =>
            p.Status == PostStatus.Scheduled &&
            p.PublishedAt.HasValue &&
            p.PublishedAt <= now);

        foreach (var post in scheduledPosts)
        {
            try
            {
                post.Status = PostStatus.Published;
                await postRepository.UpdateAsync(post);
                _logger.LogInformation("Post {PostId} has been automatically published", post.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while publishing scheduled post {PostId}", post.Id);
            }
            await unitOfWork.SaveChangesAsync();
        }

    }
}