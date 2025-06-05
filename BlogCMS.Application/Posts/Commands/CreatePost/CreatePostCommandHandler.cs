using AutoMapper;
using BlogCMS.Domain.Entities;
using BlogCMS.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BlogCMS.Application.Posts.Commands.CreatePost;

public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, Guid>
{
    private readonly IRepository<Post> _postRepository;
    private readonly IRepository<Category> _categoryRepository;
    private readonly IRepository<Tag> _tagRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;

    public CreatePostCommandHandler(
        IRepository<Post> postRepository,
        IRepository<Category> categoryRepository,
        IRepository<Tag> tagRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        UserManager<User> userManager)
    {
        _postRepository = postRepository;
        _categoryRepository = categoryRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userManager = userManager;
    }

    public async Task<Guid> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        // Validate category exists
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
        if (category == null)
        {
            throw new ArgumentException($"Category with ID {request.CategoryId} not found.");
        }

        // Create post
        var post = new Post
        {
            Title = request.Title,
            Slug = GenerateSlug(request.Title),
            Content = request.Content,
            Summary = request.Summary,
            Status = Enum.Parse<PostStatus>(request.Status),
            PublishedAt = request.PublishedAt,
            FeaturedImageUrl = request.FeaturedImageUrl,
            MetaTitle = request.MetaTitle,
            MetaDescription = request.MetaDescription,
            MetaKeywords = request.MetaKeywords,
            CategoryId = request.CategoryId
        };

        // Add tags
        foreach (var tagName in request.Tags)
        {
            var tag = await GetOrCreateTag(tagName);
            post.PostTags.Add(new PostTag { Tag = tag });
        }

        await _postRepository.AddAsync(post);
        await _unitOfWork.SaveChangesAsync();

        return post.Id;
    }

    private async Task<Tag> GetOrCreateTag(string tagName)
    {
        var slug = GenerateSlug(tagName);
        var existingTag = (await _tagRepository.FindAsync(t => t.Slug == slug))?.FirstOrDefault();

        if (existingTag != null)
        {
            return existingTag;
        }

        var newTag = new Tag
        {
            Name = tagName,
            Slug = slug
        };

        await _tagRepository.AddAsync(newTag);
        return newTag;
    }

    private static string GenerateSlug(string title)
    {
        return title.ToLower()
            .Replace(" ", "-")
            .Replace(".", "")
            .Replace(",", "")
            .Replace("!", "")
            .Replace("?", "")
            .Replace("'", "")
            .Replace("\"", "");
    }
} 