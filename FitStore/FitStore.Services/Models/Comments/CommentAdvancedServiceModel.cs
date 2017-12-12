namespace FitStore.Services.Models.Comments
{
    using AutoMapper;
    using Common.Mapping;
    using Data.Models;
    using System;

    public class CommentAdvancedServiceModel : IMapFrom<Comment>, IHaveCustomMapping
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public string Author { get; set; }

        public DateTime PublishDate { get; set; }

        public bool IsDeleted { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper
               .CreateMap<Comment, CommentAdvancedServiceModel>()
                   .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author.UserName));
        }
    }
}