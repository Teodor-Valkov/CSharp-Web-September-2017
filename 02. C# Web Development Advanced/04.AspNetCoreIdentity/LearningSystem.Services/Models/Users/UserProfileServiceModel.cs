﻿namespace LearningSystem.Services.Models.Users
{
    using AutoMapper;
    using Common.Mapping;
    using Data.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using static Common.CommonConstants;

    public class UserProfileServiceModel : IMapFrom<User>, IHaveCustomMapping
    {
        public string Username { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public DateTime Birthdate { get; set; }

        public IEnumerable<UserProfileCourseServiceModel> Courses { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            int page = default(int);

            mapper.CreateMap<User, UserProfileServiceModel>()
                .ForMember(dest => dest.Courses, opt => opt
                    .MapFrom(src => src
                        .Courses
                        .OrderBy(c => c.Course.StartDate)
                        .Skip((page - 1) * CoursePageSize)
                        .Take(CoursePageSize)
                        .Select(c => c.Course)));
        }
    }
}