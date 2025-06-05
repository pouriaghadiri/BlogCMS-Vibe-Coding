using BlogCMS.Application.Posts.DTOs;
using MediatR;

namespace BlogCMS.Application.Posts.Queries.GetPostById;

public record GetPostByIdQuery(Guid Id) : IRequest<PostDto>; 