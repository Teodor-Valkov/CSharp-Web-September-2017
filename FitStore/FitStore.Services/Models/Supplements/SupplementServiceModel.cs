namespace FitStore.Services.Models.Supplements
{
    using Common.Mapping;
    using Data.Models;
    using System;

    public class SupplementServiceModel : IMapFrom<Supplement>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public DateTime BestBeforeDate { get; set; }

        public int SubcategoryId { get; set; }

        public int ManufacturerId { get; set; }
    }
}