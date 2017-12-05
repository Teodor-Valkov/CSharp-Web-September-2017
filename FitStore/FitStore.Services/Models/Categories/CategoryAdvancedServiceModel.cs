namespace FitStore.Services.Models.Categories
{
    using AutoMapper;
    using Common.Mapping;
    using Data.Models;
    using System.Linq;

    public class CategoryAdvancedServiceModel : CategoryBasicServiceModel, IMapFrom<Category>, IHaveCustomMapping
    {
        //public int Subcategories { get; set; }

        public int Supplements { get; set; }

        public bool IsDeleted { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            //mapper
            //    .CreateMap<Category, CategoryAdvancedServiceModel>()
            //        .ForMember(dest => dest.Subcategories, opt => opt.MapFrom(src => src.Subcategories.Count));

            mapper
                .CreateMap<Category, CategoryAdvancedServiceModel>()
                    .ForMember(dest => dest.Supplements, opt => opt.MapFrom(src => src.Subcategories.Count));
        }
    }
}