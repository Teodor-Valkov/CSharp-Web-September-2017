namespace FitStore.Services.Models.Subcategories
{
    using AutoMapper;
    using Common.Mapping;
    using Data.Models;
    using Models.Categories;
    using System.Linq;

    public class SubcategoryCountServiceModel : CategoryBasicServiceModel, IMapFrom<Subcategory>, IHaveCustomMapping
    {
        public int Supplements { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper
             .CreateMap<Subcategory, SubcategoryCountServiceModel>()
                 .ForMember(dest => dest.Supplements, opt => opt.MapFrom(src => src.Supplements.Count(s => s.IsDeleted == false)));
        }
    }
}