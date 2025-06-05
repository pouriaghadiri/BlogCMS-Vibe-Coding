using System;
using System.Threading;
using System.Threading.Tasks; 
using BlogCMS.Domain.Entities;
using BlogCMS.Domain.Interfaces;
using MediatR;

namespace BlogCMS.Application.Posts.Commands.DeletePost
{
    public class DeletePostCommandHandler : IRequestHandler<DeletePostCommand, bool>
    {
        private readonly IRepository<Post> _postRepository;

        public DeletePostCommandHandler(IRepository<Post> postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<bool> Handle(DeletePostCommand request, CancellationToken cancellationToken)
        {
            var post = await _postRepository.GetByIdAsync(request.Id);
            if (post == null)
            {
                throw new Exception($"Post with ID {request.Id} not found.");
            }

            return await _postRepository.DeleteAsync(post);
        }
    }
} 