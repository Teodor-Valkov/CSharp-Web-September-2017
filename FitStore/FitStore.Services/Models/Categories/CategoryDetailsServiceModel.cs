namespace FitStore.Services.Models.Categories
{
    using AutoMapper;
    using Common.Mapping;
    using Data.Models;
    using System.Collections.Generic;
    using System.Linq;

    public class CategoryDetailsServiceModel : IMapFrom<Category>, IHaveCustomMapping
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<string> Subcategories { get; set; }

        public IEnumerable<string> Supplements { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper
                .CreateMap<Category, CategoryDetailsServiceModel>()
                    .ForMember(dest => dest.Subcategories, opt => opt.MapFrom(src => src.Subcategories.Select(s => s.Name)));

            mapper
                .CreateMap<Category, CategoryDetailsServiceModel>()
                    .ForMember(dest => dest.Supplements, opt => opt.MapFrom(src => src.Subcategories.Select(sc => sc.Supplements.Select(s => s.Name))));
        }
    }
}