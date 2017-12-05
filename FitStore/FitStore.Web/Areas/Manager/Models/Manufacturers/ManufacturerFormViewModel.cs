namespace FitStore.Web.Areas.Manager.Models.Manufacturers
{
    using System.ComponentModel.DataAnnotations;

    using static Data.DataConstants;

    public class ManufacturerFormViewModel
    {
        [Required]
        [MinLength(ManufacturerNameMinLength)]
        [MaxLength(ManufacturerNameMaxLength)]
        public string Name { get; set; }

        [Required]
        [MinLength(ManufacturerAddressMinLength)]
        [MaxLength(ManufacturerAddressMaxLength)]
        public string Address { get; set; }
    }
}