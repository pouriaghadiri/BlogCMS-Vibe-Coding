using BlogCMS.Application.Common.Models;
using BlogCMS.Application.Posts.Commands.ChangePostStatus;
using BlogCMS.Application.Posts.Commands.CreatePost;
using BlogCMS.Application.Posts.DTOs;
using BlogCMS.Application.Posts.Queries.GetPostById;
using BlogCMS.Application.Posts.Queries.GetPosts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogCMS.API.Endpoints.Posts;

public static class PostsEndpoints
{
    public static void MapPostsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/posts")
            .WithTags("Posts")
            .WithOpenApi();

        // GET /api/posts
        group.MapGet("/", async (
            IMediator mediator,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? categorySlug = null,
            [FromQuery] string? tagSlug = null,
            [FromQuery] string? status = null
        ) =>
        {
            var query = new GetPostsQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                CategorySlug = categorySlug,
                TagSlug = tagSlug,
                Status = status
            };

            var result = await mediator.Send(query);
            return Results.Ok(ApiResponse<PaginatedList<PostDto>>.Succeed(result));
        })
        .WithName("GetPosts")
        .WithSummary("Get a paginated list of posts")
        .Produces<ApiResponse<PaginatedList<PostDto>>>(200);

        // GET /api/posts/{id}
        group.MapGet("/{id}", async (
            IMediator mediator,
            Guid id
        ) =>
        {
            var query = new GetPostByIdQuery(id);
            var result = await mediator.Send(query);
            return Results.Ok(ApiResponse<PostDto>.Succeed(result));
        })
        .WithName("GetPostById")
        .WithSummary("Get a post by ID")
        .Produces<ApiResponse<PostDto>>(200)
        .Produces<ApiResponse<PostDto>>(404);

        // POST /api/posts
        group.MapPost("/", async (
            IMediator mediator,
            CreatePostCommand command
        ) =>
        {
            var result = await mediator.Send(command);
            return Results.Created($"/api/posts/{result}", ApiResponse<Guid>.Succeed(result, "Post created successfully"));
        })
        .WithName("CreatePost")
        .WithSummary("Create a new post")
        .RequireAuthorization("RequireAdminRole")
        .Produces<ApiResponse<Guid>>(201)
        .Produces<ApiResponse<Guid>>(400);

        // PUT /api/posts/{id}/status
        group.MapPut("/{id}/status", async (
            IMediator mediator,
            Guid id,
            [FromBody] ChangePostStatusCommand command
        ) =>
        {
            var newCommand = new ChangePostStatusCommand
            {
                PostId = id,
                NewStatus = command.NewStatus,
                PublishDate = command.PublishDate
            };
            var result = await mediator.Send(newCommand);
            return Results.Ok(ApiResponse<bool>.Succeed(result, "Post status updated successfully"));
        })
        .WithName("ChangePostStatus")
        .WithSummary("Change the status of a post")
        .RequireAuthorization("RequireAdminRole")
        .Produces<ApiResponse<bool>>(200)
        .Produces<ApiResponse<bool>>(400)
        .Produces<ApiResponse<bool>>(404);
    }
} 