using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper; 
using BlogCMS.Domain.Entities;
using BlogCMS.Domain.Interfaces;
using MediatR;

namespace BlogCMS.Application.Posts.Commands.UpdatePost
{
    public class UpdatePostCommandHandler : IRequestHandler<UpdatePostCommand, Guid>
    {
        private readonly IRepository<Post> _postRepository;
        private readonly IMapper _mapper;

        public UpdatePostCommandHandler(IRepository<Post> postRepository, IMapper mapper)
        {
            _postRepository = postRepository;
            _mapper = mapper;
        }

        public async Task<Guid> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
        {
            var post = await _postRepository.GetByIdAsync(request.Id);
            if (post == null)
            {
                throw new Exception($"Post with ID {request.Id} not found.");
            }

            _mapper.Map(request, post);
            await _postRepository.UpdateAsync(post);

            return post.Id;
        }
    }
} 