using System;
using MediatR;

namespace BlogCMS.Application.Posts.Commands.UpdatePost
{
    public class UpdatePostCommand : IRequest<Guid>
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string Summary { get; set; } = null!;
        public Guid CategoryId { get; set; }
    }
} 