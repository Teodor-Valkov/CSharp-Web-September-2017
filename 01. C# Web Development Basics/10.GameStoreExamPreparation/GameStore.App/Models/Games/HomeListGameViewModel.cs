namespace GameStore.App.Models.Games
{
    using Infrastructure.Mapping;
    using GameStore.Models;

    public class HomeListGameViewModel : IMapFrom<Game>
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string ThumbnailUrl { get; set; }

        public decimal Price { get; set; }

        public double Size { get; set; }

        public string VideoId { get; set; }
    }
}