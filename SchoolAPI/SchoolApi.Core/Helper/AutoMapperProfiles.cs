using AutoMapper;
using SchoolApi.Core.DTO;
using SchoolApi.Core.Models;

namespace SchoolApi.Core.Helper
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Student, StudentPostDTO>().ReverseMap();
            CreateMap<Student, StudentGetDTO>().ReverseMap();
            CreateMap<Student, StudentUpdateDTO>().ReverseMap();

        }
    }
}
