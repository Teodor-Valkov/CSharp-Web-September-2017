namespace GameStore.App.Models.Shopping
{
    using Infrastructure.Mapping;
    using GameStore.Models;

    public class CartDetailsViewModel : IMapFrom<Game>
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string ThumbnailUrl { get; set; }

        public decimal Price { get; set; }

        public string VideoId { get; set; }
    }
}