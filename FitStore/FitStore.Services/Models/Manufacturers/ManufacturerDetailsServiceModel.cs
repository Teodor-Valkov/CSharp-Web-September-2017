namespace FitStore.Services.Models.Manufacturers
{
    using AutoMapper;
    using Common.Mapping;
    using Data.Models;
    using Models.Supplements;
    using System.Collections.Generic;
    using System.Linq;

    using static Common.CommonConstants;

    public class ManufacturerDetailsServiceModel : ManufacturerBasicServiceModel, IMapFrom<Manufacturer>, IHaveCustomMapping
    {
        public IEnumerable<SupplementAdvancedServiceModel> Supplements { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            int page = default(int);

            mapper
                .CreateMap<Manufacturer, ManufacturerDetailsServiceModel>()
                    .ForMember(dest => dest.Supplements, opt => opt
                        .MapFrom(src => src.Supplements
                            .Where(s => s.IsDeleted == false)
                            .OrderBy(s => s.Name)
                            .Skip((page - 1) * SupplementPageSize)
                            .Take(SupplementPageSize)));
        }
    }
}