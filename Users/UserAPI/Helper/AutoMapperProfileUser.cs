using AutoMapper;
using UserAPI.Business.Models;
using UserAPI.DTO;

namespace UserAPI.Helper
{
    public class AutoMapperProfileUser : Profile
    {
        public AutoMapperProfileUser()
        {
            CreateMap<User, UserPostDTO>().ReverseMap();
            CreateMap<User, UserGetDTO>().ReverseMap();
            CreateMap<User, UserUpdateDTO>().ReverseMap();
        }
    }
}