namespace FitStore.Services.Models.Supplements
{
    using AutoMapper;
    using Common.Extensions;
    using Common.Mapping;
    using Data.Models;

    public class SupplementInOrderServiceModel : IMapFrom<OrderSupplements>, IHaveCustomMapping
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public string Picture { get; set; }

        public string ManufacturerName { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper
               .CreateMap<OrderSupplements, SupplementInOrderServiceModel>()
                   .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.SupplementId))
                   .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Supplement.Name))
                   .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Supplement.Price))
                   .ForMember(dest => dest.Picture, opt => opt.MapFrom(src => src.Supplement.Picture.FromByteArrayToString()))
                   .ForMember(dest => dest.ManufacturerName, opt => opt.MapFrom(src => src.Supplement.Manufacturer.Name));
        }
    }
}