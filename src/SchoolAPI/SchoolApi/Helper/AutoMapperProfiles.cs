﻿using AutoMapper;
using SchoolAPI.DTO;
using SchoolApi.Core.Models;
using SchoolApi.Core.Extensions;

namespace SchoolAPI.Helper
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