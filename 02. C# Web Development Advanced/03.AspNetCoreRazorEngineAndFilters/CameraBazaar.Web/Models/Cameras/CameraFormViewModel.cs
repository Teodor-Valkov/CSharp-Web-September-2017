namespace CameraBazaar.Web.Models.Cameras
{
    using Data.Common;
    using Data.Models.Enums;
    using Infrastructure.Validations;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class CameraFormViewModel : IValidatableObject
    {
        public Make Make { get; set; }

        [Required]
        [Model]
        [StringLength(PropertyValidationConstants.ModelMaxLength, ErrorMessage = PropertyErrorMessageConstants.ModelErrorMessage)]
        public string Model { get; set; }

        [Range(PropertyValidationConstants.PriceMinValue, PropertyValidationConstants.PriceMaxValue, ErrorMessage = PropertyErrorMessageConstants.PriceErrorMessage)]
        public decimal Price { get; set; }

        [Range(PropertyValidationConstants.QuantityMinValue, PropertyValidationConstants.QuantityMaxValue, ErrorMessage = PropertyErrorMessageConstants.QuantityErrorMessage)]
        public int Quantity { get; set; }

        [Display(Name = PropertyDisplayNameConstants.MinShutterSpeedName)]
        [Range(PropertyValidationConstants.MinShutterSpeedMinValue, PropertyValidationConstants.MinShutterSpeedMaxValue, ErrorMessage = PropertyErrorMessageConstants.MinShutterSpeedErrorMessage)]
        public int MinShutterSpeed { get; set; }

        [Display(Name = PropertyDisplayNameConstants.MaxShutterSpeedName)]
        [Range(PropertyValidationConstants.MaxShutterSpeedMinValue, PropertyValidationConstants.MaxShutterSpeedMaxValue, ErrorMessage = PropertyErrorMessageConstants.MaxShutterSpeedErrorMessage)]
        public int MaxShutterSpeed { get; set; }

        [Display(Name = PropertyDisplayNameConstants.MinISOName)]
        public MinISO MinISO { get; set; }

        [Display(Name = PropertyDisplayNameConstants.MaxISOName)]
        [Range(PropertyValidationConstants.MaxISOMinValue, PropertyValidationConstants.MaxISOMaxValue, ErrorMessage = PropertyErrorMessageConstants.MaxISOErrorMessage)]
        public int MaxISO { get; set; }

        [Display(Name = PropertyDisplayNameConstants.IsFullFrameName)]
        public bool IsFullFrame { get; set; }

        [Required]
        [Display(Name = PropertyDisplayNameConstants.LightMeteringName)]
        public IEnumerable<LightMetering> LightMeterings { get; set; }

        [Required]
        [Display(Name = PropertyDisplayNameConstants.VideoResolutionName)]
        [StringLength(PropertyValidationConstants.VideoResolutionMaxLength, ErrorMessage = PropertyErrorMessageConstants.VideoResolutionErrorMessage)]
        public string VideoResolution { get; set; }

        [Required]
        [StringLength(PropertyValidationConstants.DescriptionMaxLength, ErrorMessage = PropertyErrorMessageConstants.VideoResolutionErrorMessage)]
        public string Description { get; set; }

        [Required]
        [Display(Name = PropertyDisplayNameConstants.ImageUrlName)]
        [Image]
        [StringLength(PropertyValidationConstants.ImageUrlMaxLength, MinimumLength = PropertyValidationConstants.ImageUrlMinLength, ErrorMessage = PropertyErrorMessageConstants.ImageUrlErrorMessage)]
        public string ImageUrl { get; set; }

        public bool IsEdit { get; set; }

        // In this way the client validation is on the top of the form (MaxISO)
        // Using attribute validation property puts the client validation under the wrong input ([Model] and [Image])
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            ICollection<ValidationResult> validationResults = new List<ValidationResult>();

            if (this.MaxISO % 100 != 0)
            {
                validationResults.Add(new ValidationResult(PropertyErrorMessageConstants.MaxISOContentErrorMessage));
            }

            return validationResults;
        }
    }
}