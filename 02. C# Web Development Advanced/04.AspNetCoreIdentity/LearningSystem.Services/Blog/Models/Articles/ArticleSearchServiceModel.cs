namespace LearningSystem.Services.Blog.Models.Articles
{
    using AutoMapper;
    using Common.Mapping;
    using Data.Models;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ArticleSearchServiceModel : IMapFrom<Article>, IHaveCustomMapping
    {
        public int Id { get; set; }

        public string Title { get; set; }

        [Display(Name = "Publish Date")]
        public DateTime PublishDate { get; set; }

        public string Author { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper
                .CreateMap<Article, ArticleSearchServiceModel>()
                    .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author.Name));
        }
    }
}