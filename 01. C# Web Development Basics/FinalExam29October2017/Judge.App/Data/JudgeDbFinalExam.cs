namespace Judge.App.Data
{
    using Data.Models;
    using Microsoft.EntityFrameworkCore;

    public class JudgeDbFinalExam : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Contest> Contests { get; set; }

        public DbSet<Submission> Submissions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer($"Server=.;Database=JudgeDbFinalExam;Integrated Security=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
              .Entity<User>()
              .HasIndex(u => u.Email)
              .IsUnique();

            modelBuilder
                .Entity<Submission>()
                .HasOne(s => s.User)
                .WithMany(u => u.Submissions)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<Contest>()
                .HasOne(c => c.User)
                .WithMany(u => u.Contests)
                .HasForeignKey(s => s.UserId);

            modelBuilder
                .Entity<Contest>()
                .HasMany(c => c.Submissions)
                .WithOne(s => s.Contest)
                .HasForeignKey(s => s.ContestId);
        }
    }
}