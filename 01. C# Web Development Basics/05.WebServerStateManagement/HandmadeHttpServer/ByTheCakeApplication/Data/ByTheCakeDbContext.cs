namespace HandmadeHttpServer.ByTheCakeApplication.Data
{
    using HandmadeHttpServer.ByTheCakeApplication.Data.Models;
    using Microsoft.EntityFrameworkCore;

    public class ByTheCakeDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderProduct> OrderProduct { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=.;Database=ByTheCakeDb;Integrated Security=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<OrderProduct>()
                .HasKey(op => new { op.OrderId, op.ProductId });

            modelBuilder
                .Entity<OrderProduct>()
                .HasOne(op => op.Order)
                .WithMany(o => o.Products)
                .HasForeignKey(op => op.OrderId);

            modelBuilder
                .Entity<OrderProduct>()
                .HasOne(op => op.Product)
                .WithMany(p => p.Orders)
                .HasForeignKey(op => op.ProductId);

            modelBuilder
                .Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId);
        }
    }
}