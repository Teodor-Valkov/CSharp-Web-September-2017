namespace FitStore.Services.Models.Manufacturers
{
    using AutoMapper;
    using Common.Mapping;
    using Data.Models;

    public class ManufacturerAdvancedServiceModel : ManufacturerBasicServiceModel, IMapFrom<Manufacturer>, IHaveCustomMapping
    {
        public int Supplements { get; set; }

        public bool IsDeleted { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper
                .CreateMap<Manufacturer, ManufacturerAdvancedServiceModel>()
                    .ForMember(dest => dest.Supplements, opt => opt.MapFrom(src => src.Supplements.Count));
        }
    }
}