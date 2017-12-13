namespace FitStore.Services.Moderator.Models.Users
{
    using AutoMapper;
    using Common.Mapping;
    using Data.Models;

    public class ModeratorUserBasicServiceModel : IMapFrom<User>, IHaveCustomMapping
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public int Reviews { get; set; }

        public int Comments { get; set; }

        public bool IsRestricted { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper
             .CreateMap<User, ModeratorUserBasicServiceModel>()
                     .ForMember(dest => dest.Reviews, opt => opt.MapFrom(src => src.Reviews.Count))
                     .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments.Count));
        }
    }
}