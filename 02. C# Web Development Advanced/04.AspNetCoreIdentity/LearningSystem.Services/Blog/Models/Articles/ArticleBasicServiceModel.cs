namespace LearningSystem.Services.Blog.Models.Articles
{
    using AutoMapper;
    using Common.Extensions;
    using Common.Mapping;
    using Data.Models;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ArticleBasicServiceModel : IMapFrom<Article>, IHaveCustomMapping
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Resume { get; set; }

        [Display(Name = "Publish Date")]
        public DateTime PublishDate { get; set; }

        public string Author { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper
             .CreateMap<Article, ArticleBasicServiceModel>()
             .ForMember(dest => dest.Resume, opt => opt.MapFrom(src => src.Content.ToResume()))
             .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author.Name));
        }
    }
}