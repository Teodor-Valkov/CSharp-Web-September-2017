namespace FitStore.Services.Models.Subcategories
{
    using AutoMapper;
    using Common.Mapping;
    using Data.Models;
    using Supplements;
    using System.Collections.Generic;
    using System.Linq;

    public class SubcategoryDetailsServiceModel : SubcategoryBasicServiceModel, IMapFrom<Subcategory>, IHaveCustomMapping
    {
        public string CategoryId { get; set; }

        public string CategoryName { get; set; }

        public bool IsDeleted { get; set; }

        public IEnumerable<SupplementAdvancedServiceModel> Supplements { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper
                .CreateMap<Subcategory, SubcategoryDetailsServiceModel>()
                    .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                    .ForMember(dest => dest.Supplements, opt => opt.MapFrom(src => src.Supplements.Where(s => s.IsDeleted == false)));
        }
    }
}