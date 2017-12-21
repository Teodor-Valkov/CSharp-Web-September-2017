namespace FitStore.Web.Areas.Manager.Models.Manufacturers
{
    using System.ComponentModel.DataAnnotations;

    using static Common.CommonMessages;
    using static Data.DataConstants;

    public class ManufacturerFormViewModel
    {
        [Required]
        [StringLength(ManufacturerNameMaxLength, ErrorMessage = FieldLengthErrorMessage, MinimumLength = ManufacturerNameMinLength)]
        public string Name { get; set; }

        [Required]
        [StringLength(ManufacturerAddressMaxLength, ErrorMessage = FieldLengthErrorMessage, MinimumLength = ManufacturerAddressMinLength)]
        public string Address { get; set; }
    }
}