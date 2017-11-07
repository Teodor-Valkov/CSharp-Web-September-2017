namespace _03.FootballBetting.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Color
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<Team> HomeTeamColors { get; set; } = new List<Team>();

        public ICollection<Team> AwayTeamColors { get; set; } = new List<Team>();
    }
}