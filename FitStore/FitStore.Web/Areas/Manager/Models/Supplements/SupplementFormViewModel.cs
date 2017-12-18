namespace FitStore.Web.Areas.Manager.Models.Supplements
{
    using FitStore.Web.Infrastructure.Validation.ValidationAttributes;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using static Common.CommonConstants;
    using static Data.DataConstants;

    public class SupplementFormViewModel
    {
        [Required]
        [MinLength(SupplementNameMinLength)]
        [MaxLength(SupplementNameMaxLength)]
        public string Name { get; set; }

        [Required]
        [MinLength(SupplementDescriptionMinLength)]
        [MaxLength(SupplementDescriptionMaxLength)]
        public string Description { get; set; }

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Picture]
        [DataType(DataType.Upload)]
        public IFormFile Picture { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = SupplementBestBeforeDateName)]
        public DateTime BestBeforeDate { get; set; }

        [Display(Name = SubcategoryEntity)]
        public int SubcategoryId { get; set; }

        public IEnumerable<SelectListItem> Subcategories { get; set; }

        [Display(Name = ManufacturerEntity)]
        public int ManufacturerId { get; set; }

        public IEnumerable<SelectListItem> Manufacturers { get; set; }
    }
}