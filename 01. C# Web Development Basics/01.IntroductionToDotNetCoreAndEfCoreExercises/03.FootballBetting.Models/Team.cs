namespace _03.FootballBetting.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Team
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(3)]
        public string Initials { get; set; }

        public decimal Budget { get; set; }

        public int TownId { get; set; }

        public Town Town { get; set; }

        public int PrimaryKitColorId { get; set; }

        public Color PrimaryKitColor { get; set; }

        public int SecondaryKitColorId { get; set; }

        public Color SecondaryKitColor { get; set; }

        public ICollection<Game> HomeGames { get; set; } = new List<Game>();

        public ICollection<Game> AwayGames { get; set; } = new List<Game>();

        public ICollection<Player> Players { get; set; } = new List<Player>();
    }
}