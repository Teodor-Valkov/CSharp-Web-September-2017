namespace LearningSystem.Services.Models.Users
{
    using AutoMapper;
    using Common.Mapping;
    using Data.Models;
    using System;

    public class UserSearchServiceModel : IMapFrom<User>, IHaveCustomMapping
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public string Name { get; set; }

        public DateTime Birthdate { get; set; }

        public int Courses { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper
                .CreateMap<User, UserSearchServiceModel>()
                    .ForMember(dest => dest.Courses, opt => opt.MapFrom(src => src.Courses.Count));
        }
    }
}