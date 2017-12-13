namespace FitStore.Services.Models.Orders
{
    using AutoMapper;
    using Common.Mapping;
    using Data.Models;
    using Models.Supplements;
    using System;
    using System.Collections.Generic;

    public class OrderDetailsServiceModel : IMapFrom<Order>, IHaveCustomMapping
    {
        public int Id { get; set; }

        public decimal TotalPrice { get; set; }

        public DateTime PurchaseDate { get; set; }

        public IEnumerable<SupplementInOrderServiceModel> Supplements { get; set; }

        public virtual void ConfigureMapping(Profile mapper)
        {
            mapper
                .CreateMap<Order, OrderDetailsServiceModel>()
                    .ForMember(dest => dest.Supplements, opt => opt.MapFrom(src => src.Supplements));
        }
    }
}