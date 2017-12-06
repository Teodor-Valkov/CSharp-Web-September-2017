namespace FitStore.Services.Models.Categories
{
    using AutoMapper;
    using Common.Mapping;
    using Data.Models;
    using FitStore.Services.Models.Supplements;
    using Subcategories;
    using System.Collections.Generic;
    using System.Linq;

    public class CategoryDetailsServiceModel : CategoryBasicServiceModel, IMapFrom<Category>, IHaveCustomMapping
    {
        public IEnumerable<SubcategoryCountServiceModel> Subcategories { get; set; }

        public IEnumerable<SupplementAdvancedServiceModel> Supplements { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper
                .CreateMap<Category, CategoryDetailsServiceModel>()
                    .ForMember(dest => dest.Subcategories, opt => opt.MapFrom(src => src.Subcategories))
                    .ForMember(dest => dest.Supplements, opt => opt.MapFrom(src => src.Subcategories.SelectMany(s => s.Supplements)));
        }
    }
}