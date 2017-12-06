namespace FitStore.Services.Models.Categories
{
    using AutoMapper;
    using Common.Mapping;
    using Data.Models;
    using Subcategories;
    using System.Collections.Generic;

    public class CategoryDetailsServiceModel : CategoryBasicServiceModel, IMapFrom<Category>, IHaveCustomMapping
    {
        public IEnumerable<SubcategoryBasicServiceModel> Subcategories { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper
                .CreateMap<Category, CategoryDetailsServiceModel>()
                    .ForMember(dest => dest.Subcategories, opt => opt.MapFrom(src => src.Subcategories));
        }
    }
}