namespace _03.FootballBetting.Models
{
    public class PlayerStatistics
    {
        public int GameId { get; set; }

        public Game Game { get; set; }

        public int PlayerId { get; set; }

        public Player Player { get; set; }

        public int ScoredGoals { get; set; }

        public int Assists { get; set; }

        public int PlayedMinutes { get; set; }
    }
}