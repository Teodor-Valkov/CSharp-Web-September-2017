namespace _04.BankSystem.Data
{
    using _04.BankSystem.Models;
    using Microsoft.EntityFrameworkCore;

    public class BankSystemDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<SavingsAccount> SavingAccounts { get; set; }

        public DbSet<CheckingAccount> CheckingAccounts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=.;Database=BankSystemDb;Integrated Security=true;");

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<SavingsAccount>()
                .HasOne(sa => sa.User)
                .WithMany(u => u.SavingAccounts)
                .HasForeignKey(sa => sa.UserId);

            modelBuilder.Entity<CheckingAccount>()
                .HasOne(ca => ca.User)
                .WithMany(u => u.CheckingAccounts)
                .HasForeignKey(ca => ca.UserId);

            base.OnModelCreating(modelBuilder);
        }
    }
}