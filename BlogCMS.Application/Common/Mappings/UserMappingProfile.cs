using AutoMapper;
using BlogCMS.Application.Users.DTOs;
using BlogCMS.Domain.Entities;

namespace BlogCMS.Application.Common.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Roles, opt => opt.Ignore()); // Roles will be set manually in handlers
    }
} 