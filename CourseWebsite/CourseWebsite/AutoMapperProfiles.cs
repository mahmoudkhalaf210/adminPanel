using AutoMapper;
using CourseWebsite.Data;
using CourseWebsite.DTOs.Account;
using Microsoft.Extensions.Hosting;

namespace CourseWebsite
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<ApplicationUser, RegisterUserDTO>();
            CreateMap<RegisterUserDTO, ApplicationUser>();
            CreateMap<ApplicationUser, GetUserDTO>();
            CreateMap<GetUserDTO, ApplicationUser>();

        }
    }
}

