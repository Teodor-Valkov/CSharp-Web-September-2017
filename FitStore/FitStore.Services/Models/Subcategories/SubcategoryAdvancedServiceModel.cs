namespace FitStore.Services.Models.Subcategories
{
    using AutoMapper;
    using Common.Mapping;
    using Data.Models;

    public class SubcategoryAdvancedServiceModel : SubcategoryBasicServiceModel, IMapFrom<Subcategory>, IHaveCustomMapping
    {
        public int Supplements { get; set; }

        public bool IsDeleted { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper
                .CreateMap<Subcategory, SubcategoryAdvancedServiceModel>()
                    .ForMember(dest => dest.Supplements, opt => opt.MapFrom(src => src.Supplements.Count));
        }
    }
}