namespace _05.ShopHierarchy.Data
{
    using Microsoft.EntityFrameworkCore;
    using Models;

    public class MyDbContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }

        public DbSet<Salesman> Salesmen { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<Review> Reviews { get; set; }

        public DbSet<Item> Items { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=.;Database=ShopHierarchyDb;Integrated Security=true;");

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>()
                .HasOne(c => c.Salesman)
                .WithMany(s => s.Customers)
                .HasForeignKey(c => c.SalesmanId);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Customer)
                .WithMany(c => c.Reviews)
                .HasForeignKey(r => r.CustomerId);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Item)
                .WithMany(i => i.Reviews)
                .HasForeignKey(r => r.ItemId);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId);

            modelBuilder.Entity<ItemOrder>()
                .HasKey(io => new { io.ItemId, io.OrderId });

            modelBuilder.Entity<ItemOrder>()
                .HasOne(io => io.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(io => io.OrderId);

            modelBuilder.Entity<ItemOrder>()
                .HasOne(io => io.Item)
                .WithMany(i => i.Orders)
                .HasForeignKey(io => io.ItemId);

            base.OnModelCreating(modelBuilder);
        }
    }
}