namespace CameraBazaar.Services.Models.Cameras
{
    using Data.Common;
    using Data.Models.Enums;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class CameraFormServiceModel
    {
        public Make Make { get; set; }

        public string Model { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        [Display(Name = PropertyDisplayNameConstants.MinShutterSpeedName)]
        public int MinShutterSpeed { get; set; }

        [Display(Name = PropertyDisplayNameConstants.MaxShutterSpeedName)]
        public int MaxShutterSpeed { get; set; }

        [Display(Name = PropertyDisplayNameConstants.MinISOName)]
        public MinISO MinISO { get; set; }

        [Display(Name = PropertyDisplayNameConstants.MaxISOName)]
        public int MaxISO { get; set; }

        [Display(Name = PropertyDisplayNameConstants.IsFullFrameName)]
        public bool IsFullFrame { get; set; }

        [Display(Name = PropertyDisplayNameConstants.LightMeteringName)]
        public IEnumerable<LightMetering> LightMeterings { get; set; }

        [Display(Name = PropertyDisplayNameConstants.VideoResolutionName)]
        public string VideoResolution { get; set; }

        public string Description { get; set; }

        [Display(Name = PropertyDisplayNameConstants.ImageUrlName)]
        public string ImageUrl { get; set; }
    }
}