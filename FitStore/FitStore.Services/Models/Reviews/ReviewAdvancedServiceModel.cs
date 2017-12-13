namespace FitStore.Services.Models.Reviews
{
    using AutoMapper;
    using Common.Extensions;
    using Common.Mapping;
    using Data.Models;

    public class ReviewAdvancedServiceModel : ReviewBasicServiceModel, IMapFrom<Review>, IHaveCustomMapping
    {
        public int SupplementId { get; set; }

        public string SupplementName { get; set; }

        public string Picture { get; set; }

        public string Author { get; set; }

        public bool IsDeleted { get; set; }

        public virtual void ConfigureMapping(Profile mapper)
        {
            mapper
                .CreateMap<Review, ReviewAdvancedServiceModel>()
                    .ForMember(dest => dest.SupplementId, opt => opt.MapFrom(src => src.SupplementId))
                    .ForMember(dest => dest.SupplementName, opt => opt.MapFrom(src => src.Supplement.Name))
                    .ForMember(dest => dest.Picture, opt => opt.MapFrom(src => src.Supplement.Picture.FromByteArrayToString()))
                    .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author.UserName));
        }
    }
}