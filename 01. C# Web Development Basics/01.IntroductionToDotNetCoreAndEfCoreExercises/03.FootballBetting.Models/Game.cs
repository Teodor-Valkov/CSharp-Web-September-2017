﻿namespace _03.FootballBetting.Models
{
    using System;
    using System.Collections.Generic;

    public class Game
    {
        public int Id { get; set; }

        public int HomeTeamId { get; set; }

        public Team HomeTeam { get; set; }

        public int AwayTeamId { get; set; }

        public Team AwayTeam { get; set; }

        public int HomeGoals { get; set; }

        public int AwayGoals { get; set; }

        public DateTime StartingTime { get; set; }

        public double HomeTeamWinBetRate { get; set; }

        public double AwayTeamWinBetRate { get; set; }

        public double DrawGameBetRate { get; set; }

        public int RoundId { get; set; }

        public Round Round { get; set; }

        public int CompetitionId { get; set; }

        public Competition Competition { get; set; }

        public ICollection<BetGame> Bets { get; set; } = new List<BetGame>();

        public ICollection<PlayerStatistics> Players { get; set; } = new List<PlayerStatistics>();
    }
}