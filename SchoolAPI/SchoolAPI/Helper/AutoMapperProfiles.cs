using AutoMapper;
using SchoolAPI.DTO;
using SchoolAPI.Models;

namespace SchoolAPI.Helper
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Student, StudentPostDTO>().ReverseMap();
            CreateMap<Student, StudentGetDTO>().ReverseMap();

        }
    }
}
