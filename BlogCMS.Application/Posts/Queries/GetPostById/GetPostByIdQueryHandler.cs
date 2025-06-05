using AutoMapper;
using BlogCMS.Application.Posts.DTOs;
using BlogCMS.Domain.Entities;
using BlogCMS.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlogCMS.Application.Posts.Queries.GetPostById;

public class GetPostByIdQueryHandler : IRequestHandler<GetPostByIdQuery, PostDto>
{
    private readonly IRepository<Post> _postRepository;
    private readonly IMapper _mapper;

    public GetPostByIdQueryHandler(IRepository<Post> postRepository, IMapper mapper)
    {
        _postRepository = postRepository;
        _mapper = mapper;
    }

    public async Task<PostDto> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetByIdAsync(request.Id);
        if (post == null)
        {
            throw new KeyNotFoundException($"Post with ID {request.Id} not found.");
        }

        return _mapper.Map<PostDto>(post);
    }
} 