namespace GameStore.GameStoreApplication.ViewModels.Game
{
    using System;

    public class DetailsGameViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public double Size { get; set; }

        public string VideoId { get; set; }

        public DateTime? ReleaseDate { get; set; }
    }
}