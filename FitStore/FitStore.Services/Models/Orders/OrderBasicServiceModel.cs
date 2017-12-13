namespace FitStore.Services.Models.Orders
{
    using AutoMapper;
    using Common.Mapping;
    using Data.Models;
    using System;
    using System.Linq;

    public class OrderBasicServiceModel : IMapFrom<Order>, IHaveCustomMapping
    {
        public int Id { get; set; }

        public decimal TotalPrice { get; set; }

        public DateTime PurchaseDate { get; set; }

        public int Supplements { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper
                .CreateMap<Order, OrderBasicServiceModel>()
                  .ForMember(dest => dest.Supplements, opt => opt.MapFrom(src => src.Supplements.Sum(s => s.Quantity)));
        }
    }
}