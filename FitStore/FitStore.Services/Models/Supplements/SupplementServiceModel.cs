namespace FitStore.Services.Models.Supplements
{
    using Common.Mapping;
    using Data.Models;
    using System;
    using System.ComponentModel.DataAnnotations;

    using static Common.CommonConstants;
    using static Data.DataConstants;

    public class SupplementServiceModel : IMapFrom<Supplement>
    {
        //[Required]
        //[MinLength(SupplementNameMinLength)]
        //[MaxLength(SupplementNameMaxLength)]
        public string Name { get; set; }

        //[Required]
        //[MinLength(SupplementDescriptionMinLength)]
        //[MaxLength(SupplementDescriptionMaxLength)]
        public string Description { get; set; }

        //[Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        //[Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        //[Display(Name = SupplementBestBeforeDateName)]
        //[DataType(DataType.Date)]
        public DateTime BestBeforeDate { get; set; }

        //[Display(Name = SubcategoryEntity)]
        public int SubcategoryId { get; set; }

        //[Display(Name = ManufacturerEntity)]
        public int ManufacturerId { get; set; }
    }
}