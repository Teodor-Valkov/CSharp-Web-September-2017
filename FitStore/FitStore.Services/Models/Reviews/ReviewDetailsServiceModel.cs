namespace FitStore.Services.Models.Reviews
{
    using AutoMapper;
    using Common.Extensions;
    using Common.Mapping;
    using Data.Models;

    public class ReviewDetailsServiceModel : ReviewAdvancedServiceModel, IMapFrom<Review>, IHaveCustomMapping
    {
        public int ManufacturerId { get; set; }

        public string ManufacturerName { get; set; }

        public override void ConfigureMapping(Profile mapper)
        {
            mapper
               .CreateMap<Review, ReviewDetailsServiceModel>()
                   .ForMember(dest => dest.SupplementId, opt => opt.MapFrom(src => src.SupplementId))
                   .ForMember(dest => dest.SupplementName, opt => opt.MapFrom(src => src.Supplement.Name))
                   .ForMember(dest => dest.Picture, opt => opt.MapFrom(src => src.Supplement.Picture.FromByteArrayToString()))
                   .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author.UserName))
                   .ForMember(dest => dest.ManufacturerId, opt => opt.MapFrom(src => src.Supplement.ManufacturerId))
                   .ForMember(dest => dest.ManufacturerName, opt => opt.MapFrom(src => src.Supplement.Manufacturer.Name));
        }
    }
}