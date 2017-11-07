namespace GameStore.GameStoreApplication.ViewModels.Game
{
    public class ListGamesViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public string VideoId { get; set; }

        public double Size { get; set; }

        public decimal Price { get; set; }
    }
}