﻿namespace _03.FootballBetting.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Player
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int Number { get; set; }

        public bool IsInjured { get; set; }

        public string PositionId { get; set; }

        public Position Position { get; set; }

        public int TeamId { get; set; }

        public Team Team { get; set; }

        public ICollection<PlayerStatistics> Games { get; set; } = new List<PlayerStatistics>();
    }
}