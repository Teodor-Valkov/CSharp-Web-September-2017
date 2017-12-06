namespace FitStore.Services.Models.Manufacturers
{
    using AutoMapper;
    using Common.Mapping;
    using Data.Models;
    using System.Collections.Generic;
    using System.Linq;

    public class ManufacturerDetailsServiceModel : ManufacturerBasicServiceModel, IMapFrom<Manufacturer>, IHaveCustomMapping
    {
        public IEnumerable<string> Supplements { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper
                .CreateMap<Manufacturer, ManufacturerDetailsServiceModel>()
                    .ForMember(dest => dest.Supplements, opt => opt.MapFrom(src => src.Supplements.Select(s => s.Name)));
        }
    }
}