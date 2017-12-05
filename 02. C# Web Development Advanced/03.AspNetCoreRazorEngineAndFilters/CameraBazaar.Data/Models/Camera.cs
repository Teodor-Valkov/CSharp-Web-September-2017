namespace CameraBazaar.Data.Models
{
    using Common;
    using Enums;
    using System.ComponentModel.DataAnnotations;

    public class Camera
    {
        public int Id { get; set; }

        public Make Make { get; set; }

        [Required]
        [MaxLength(PropertyValidationConstants.ModelMaxLength)]
        public string Model { get; set; }

        [Range(PropertyValidationConstants.PriceMinValue, PropertyValidationConstants.PriceMaxValue)]
        public decimal Price { get; set; }

        [Range(PropertyValidationConstants.QuantityMinValue, PropertyValidationConstants.QuantityMaxValue)]
        public int Quantity { get; set; }

        [Range(PropertyValidationConstants.MinShutterSpeedMinValue, PropertyValidationConstants.MinShutterSpeedMaxValue)]
        public int MinShutterSpeed { get; set; }

        [Range(PropertyValidationConstants.MaxShutterSpeedMinValue, PropertyValidationConstants.MaxShutterSpeedMaxValue)]
        public int MaxShutterSpeed { get; set; }

        public MinISO MinISO { get; set; }

        [Range(PropertyValidationConstants.MaxISOMinValue, PropertyValidationConstants.MaxISOMaxValue)]
        public int MaxISO { get; set; }

        public bool IsFullFrame { get; set; }

        public LightMetering LightMetering { get; set; }

        [Required]
        [StringLength(PropertyValidationConstants.VideoResolutionMaxLength)]
        public string VideoResolution { get; set; }

        [Required]
        [StringLength(PropertyValidationConstants.DescriptionMaxLength)]
        public string Description { get; set; }

        [Required]
        [StringLength(PropertyValidationConstants.ImageUrlMaxLength, MinimumLength = PropertyValidationConstants.ImageUrlMinLength)]
        public string ImageUrl { get; set; }

        public string UserId { get; set; }

        public User User { get; set; }
    }
}