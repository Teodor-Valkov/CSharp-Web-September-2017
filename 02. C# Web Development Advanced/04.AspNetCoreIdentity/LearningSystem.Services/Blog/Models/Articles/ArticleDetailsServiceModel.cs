namespace LearningSystem.Services.Blog.Models.Articles
{
    using AutoMapper;
    using Common.Mapping;
    using Data.Models;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ArticleDetailsServiceModel : IMapFrom<Article>, IHaveCustomMapping
    {
        public string Title { get; set; }

        public string Content { get; set; }

        [Display(Name = "Publish Date")]
        public DateTime PublishDate { get; set; }

        public string Author { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper
            .CreateMap<Article, ArticleDetailsServiceModel>()
            .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author.Name));
        }
    }
}