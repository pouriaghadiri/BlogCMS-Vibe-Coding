using System;
using MediatR;

namespace BlogCMS.Application.Posts.Commands.DeletePost
{
    public class DeletePostCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }
} 