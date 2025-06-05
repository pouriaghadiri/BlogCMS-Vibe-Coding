using AutoMapper;
using BlogCMS.Application.Posts.DTOs;
using BlogCMS.Domain.Entities;
using BlogCMS.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlogCMS.Application.Posts.Queries.GetPosts;

public class GetPostsQueryHandler : IRequestHandler<GetPostsQuery, PaginatedList<PostDto>>
{
    private readonly IRepository<Post> _postRepository;
    private readonly IMapper _mapper;

    public GetPostsQueryHandler(IRepository<Post> postRepository, IMapper mapper)
    {
        _postRepository = postRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedList<PostDto>> Handle(GetPostsQuery request, CancellationToken cancellationToken)
    {
        var query = _postRepository.GetAllAsync().Result.AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(p => p.Title.Contains(request.SearchTerm) || 
                                   p.Content.Contains(request.SearchTerm) ||
                                   p.Summary.Contains(request.SearchTerm));
        }

        if (!string.IsNullOrWhiteSpace(request.CategorySlug))
        {
            query = query.Where(p => p.Category.Slug == request.CategorySlug);
        }

        if (!string.IsNullOrWhiteSpace(request.TagSlug))
        {
            query = query.Where(p => p.PostTags.Any(pt => pt.Tag.Slug == request.TagSlug));
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (Enum.TryParse<PostStatus>(request.Status, out var status))
            {
                query = query.Where(p => p.Status == status);
            }
        }

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        // Map to DTOs
        var dtos = _mapper.Map<List<PostDto>>(items);

        return new PaginatedList<PostDto>(dtos, totalCount, request.PageNumber, request.PageSize);
    }
} 