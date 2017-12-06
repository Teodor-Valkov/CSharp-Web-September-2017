namespace FitStore.Web.Models.Subcategories
{
    using AutoMapper;
    using Common.Mapping;
    using Data.Models;
    using Pagination;
    using Services.Models.Subcategories;

    public class SubcategoryPageViewModel : PagingElementsViewModel<SubcategoryAdvancedServiceModel>, IMapFrom<Category>, IHaveCustomMapping
    {
        public string Category { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper
              .CreateMap<Subcategory, SubcategoryPageViewModel>()
                  .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.Name));
        }
    }
}