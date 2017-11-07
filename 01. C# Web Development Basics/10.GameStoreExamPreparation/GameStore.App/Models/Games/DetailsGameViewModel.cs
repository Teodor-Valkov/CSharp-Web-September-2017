namespace GameStore.App.Models.Games
{
    using Infrastructure.Mapping;
    using GameStore.Models;
    using System;

    public class GameDetailsViewModel : IMapFrom<Game>
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public double Size { get; set; }

        public string VideoId { get; set; }

        public DateTime ReleaseDate { get; set; }
    }
}