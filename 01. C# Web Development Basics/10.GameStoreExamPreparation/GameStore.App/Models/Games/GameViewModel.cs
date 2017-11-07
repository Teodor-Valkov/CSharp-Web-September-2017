namespace GameStore.App.Models.Games
{
    using Infrastructure.Mapping;
    using Infrastructure.Validation;
    using Infrastructure.Validation.Games;
    using GameStore.Models;
    using System;

    public class GameAdminViewModel : IMapFrom<Game>
    {
        [Required]
        [Title]
        public string Title { get; set; }

        [Required]
        [Description]
        public string Description { get; set; }

        [ThumbnailUrl]
        public string ThumbnailUrl { get; set; }

        [Price]
        public decimal Price { get; set; }

        [Size]
        public double Size { get; set; }

        [Required]
        [VideoId]
        public string VideoId { get; set; }

        public DateTime ReleaseDate { get; set; }
    }
}