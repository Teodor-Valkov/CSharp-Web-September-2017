namespace GameStore.GameStoreApplication.ViewModels.Admin
{
    using GameStore.GameStoreApplication.Common;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class AdminAddGameViewModel
    {
        [Required]
        [MinLength(ValidationConstants.Game.TitleMinLength, ErrorMessage = ValidationConstants.InvalidMinLengthErrorMessage)]
        [MaxLength(ValidationConstants.Game.TitleMaxLength, ErrorMessage = ValidationConstants.InvalidMaxLengthErrorMessage)]
        public string Title { get; set; }

        [Required]
        [MinLength(ValidationConstants.Game.DescriptionMinLength, ErrorMessage = ValidationConstants.InvalidMinLengthErrorMessage)]
        public string Description { get; set; }

        [Display(Name = "Thumbnail")]
        public string ImageUrl { get; set; }

        public double Size { get; set; }

        public decimal Price { get; set; }

        [Required]
        [Display(Name = "YouTube Video URL")]
        [MinLength(ValidationConstants.Game.VideoIdLength, ErrorMessage = ValidationConstants.InvalidExactLengthErrorMessage)]
        [MaxLength(ValidationConstants.Game.VideoIdLength, ErrorMessage = ValidationConstants.InvalidExactLengthErrorMessage)]
        public string VideoId { get; set; }

        [Required]
        [Display(Name = "Release Date")]
        public DateTime? ReleaseDate { get; set; }
    }
}