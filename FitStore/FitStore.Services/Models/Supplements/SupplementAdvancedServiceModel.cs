namespace FitStore.Services.Models.Supplements
{
    using AutoMapper;
    using Common.Mapping;
    using Data.Models;
    using FitStore.Common.Extensions;

    public class SupplementAdvancedServiceModel : SupplementBasicServiceModel, IMapFrom<Supplement>, IHaveCustomMapping
    {
        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public string Picture { get; set; }

        public int ManufacturerId { get; set; }

        public string ManufacturerName { get; set; }

        public bool IsDeleted { get; set; }

        public virtual void ConfigureMapping(Profile mapper)
        {
            mapper
                .CreateMap<Supplement, SupplementAdvancedServiceModel>()
                    .ForMember(dest => dest.Picture, opt => opt.MapFrom(src => src.Picture.FromByteArrayToString()))
                    .ForMember(dest => dest.ManufacturerId, opt => opt.MapFrom(src => src.Manufacturer.Id))
                    .ForMember(dest => dest.ManufacturerName, opt => opt.MapFrom(src => src.Manufacturer.Name));
        }
    }
}