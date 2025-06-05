using BlogCMS.Application.Common.Models;
using BlogCMS.Application.Users.Commands.ChangePassword;
using BlogCMS.Application.Users.Commands.CreateUser;
using BlogCMS.Application.Users.Commands.UpdateCurrentUser;
using BlogCMS.Application.Users.DTOs;
using BlogCMS.Application.Users.Queries.GetCurrentUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogCMS.API.Endpoints.Users;

public static class UsersEndpoints
{
    public static void MapUsersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users")
            .WithTags("Users")
            .WithOpenApi();

        // POST /api/users
        group.MapPost("/", async (
            [FromBody] CreateUserCommand command,
            IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return Results.Created($"/api/users/{result.Id}", ApiResponse<UserDto>.Succeed(result, "User created successfully"));
        })
        .WithName("CreateUser")
        .WithSummary("Create a new user")
        .RequireAuthorization("RequireAdminRole")
        .Produces<ApiResponse<UserDto>>(201)
        .Produces<ApiResponse<UserDto>>(400);

        // GET /api/users/me
        group.MapGet("/me", async (
            IMediator mediator) =>
        {
            var query = new GetCurrentUserQuery();
            var result = await mediator.Send(query);
            return Results.Ok(ApiResponse<UserDto>.Succeed(result));
        })
        .WithName("GetCurrentUser")
        .WithSummary("Get the current user's information")
        .RequireAuthorization()
        .Produces<ApiResponse<UserDto>>(200);

        // PUT /api/users/me
        group.MapPut("/me", async (
            [FromBody] UpdateUserDto user,
            IMediator mediator) =>
        {
            var command = new UpdateCurrentUserCommand(user);
            var result = await mediator.Send(command);
            return Results.Ok(ApiResponse<UserDto>.Succeed(result, "User updated successfully"));
        })
        .WithName("UpdateCurrentUser")
        .WithSummary("Update the current user's information")
        .RequireAuthorization()
        .Produces<ApiResponse<UserDto>>(200)
        .Produces<ApiResponse<UserDto>>(400);

        // POST /api/users/me/change-password
        group.MapPost("/me/change-password", async (
            [FromBody] ChangePasswordDto changePassword,
            IMediator mediator) =>
        {
            var command = new ChangePasswordCommand(changePassword);
            var result = await mediator.Send(command);
            return Results.Ok(ApiResponse<bool>.Succeed(result, "Password changed successfully"));
        })
        .WithName("ChangePassword")
        .WithSummary("Change the current user's password")
        .RequireAuthorization()
        .Produces<ApiResponse<bool>>(200)
        .Produces<ApiResponse<bool>>(400);
    }
} 