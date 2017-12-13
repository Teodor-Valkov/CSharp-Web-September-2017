namespace FitStore.Services.Models.Supplements
{
    using AutoMapper;
    using Common.Extensions;
    using Common.Mapping;
    using Data.Models;

    public class SupplementInCartServiceModel : IMapFrom<Supplement>, IHaveCustomMapping
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public string Picture { get; set; }

        public string ManufacturerName { get; set; }

        public virtual void ConfigureMapping(Profile mapper)
        {
            mapper
                .CreateMap<Supplement, SupplementInCartServiceModel>()
                    .ForMember(dest => dest.Picture, opt => opt.MapFrom(src => src.Picture.FromByteArrayToString()))
                    .ForMember(dest => dest.ManufacturerName, opt => opt.MapFrom(src => src.Manufacturer.Name));
        }
    }
}