namespace GameStore.GameStoreApplication.ViewModels.Admin
{
    using GameStore.GameStoreApplication.Utilities;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class AdminDetailsGameViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Display(Name = "Thumbnail")]
        [Image]
        public string ImageUrl { get; set; }

        public double Size { get; set; }

        public decimal Price { get; set; }

        [Required]
        [Display(Name = "YouTube Video URL")]
        public string VideoId { get; set; }

        [Required]
        [Display(Name = "Release Date")]
        public DateTime? ReleaseDate { get; set; }
    }
}