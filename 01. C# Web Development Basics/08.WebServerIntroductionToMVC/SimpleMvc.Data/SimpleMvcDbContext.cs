namespace SimpleMvc.Data
{
    using Microsoft.EntityFrameworkCore;
    using Models;

    public class SimpleMvcDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Note> Notes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=.;Database=SimpleMvcDb;Integrated Security=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Note>()
                .HasOne(n => n.Owner)
                .WithMany(u => u.Notes)
                .HasForeignKey(n => n.OwnerId);
        }
    }
}