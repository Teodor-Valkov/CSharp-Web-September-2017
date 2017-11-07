namespace _03.FootballBetting.Data
{
    using _03.FootballBetting.Data.Configurations;
    using _03.FootballBetting.Models;
    using Microsoft.EntityFrameworkCore;

    public class FootballBettingDbContext : DbContext
    {
        public DbSet<Bet> Bets { get; set; }

        public DbSet<Color> Colors { get; set; }

        public DbSet<Competition> Competitions { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<Continent> Continents { get; set; }

        public DbSet<Game> Games { get; set; }

        public DbSet<Player> Players { get; set; }

        public DbSet<Position> Positions { get; set; }

        public DbSet<Round> Rounds { get; set; }

        public DbSet<PlayerStatistics> Statistics { get; set; }

        public DbSet<Team> Teams { get; set; }

        public DbSet<Town> Towns { get; set; }

        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=.;Database=FootballBettingDb;Integrated Security=true;");

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Entity configurations are in folder Configurations, separated in different classes

            modelBuilder.ApplyConfiguration(new BetConfiguration());

            modelBuilder.ApplyConfiguration(new BetGameConfiguration());

            modelBuilder.ApplyConfiguration(new CountryContinentConfiguration());

            modelBuilder.ApplyConfiguration(new GameConfiguration());

            modelBuilder.ApplyConfiguration(new PlayerConfiguration());

            modelBuilder.ApplyConfiguration(new PlayerStatisticsConfiguration());

            modelBuilder.ApplyConfiguration(new TeamConfiguration());

            modelBuilder.ApplyConfiguration(new TownConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}