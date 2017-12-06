namespace FitStore.Services.Models.Categories
{
    using AutoMapper;
    using Common.Mapping;
    using Data.Models;
    using FitStore.Services.Models.Subcategories;
    using System.Collections.Generic;
    using System.Linq;

    public class CategoryAdvancedServiceModel : CategoryBasicServiceModel, IMapFrom<Category>, IHaveCustomMapping
    {
        public IEnumerable<SubcategoryBasicServiceModel> Subcategories { get; set; }

        public int Supplements { get; set; }

        public bool IsDeleted { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper
                .CreateMap<Category, CategoryAdvancedServiceModel>()
                    .ForMember(dest => dest.Subcategories, opt => opt.MapFrom(src => src.Subcategories))
                    .ForMember(dest => dest.Supplements, opt => opt.MapFrom(src => src.Subcategories.Sum(sc => sc.Supplements.Count)));
        }
    }
}