namespace GameStore.Data
{
    using Microsoft.EntityFrameworkCore;
    using Models;

    public class GameStoreDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Game> Games { get; set; }

        public DbSet<Order> Orders { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=.;Database=GameStoreDb;Integrated Security=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Order>()
                .HasKey(o => new { o.UserId, o.GameId });

            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Games)
                .HasForeignKey(o => o.UserId);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Game)
                .WithMany(g => g.Users)
                .HasForeignKey(o => o.GameId);

            modelBuilder
                .Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}