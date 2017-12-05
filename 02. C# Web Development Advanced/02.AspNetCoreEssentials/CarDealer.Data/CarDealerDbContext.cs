namespace CarDealer.Data
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Models;

    public class CarDealerDbContext : IdentityDbContext<User>
    {
        public DbSet<Car> Cars { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Part> Parts { get; set; }

        public DbSet<Log> Logs { get; set; }

        public DbSet<Sale> Sales { get; set; }

        public DbSet<Supplier> Suppliers { get; set; }

        public CarDealerDbContext(DbContextOptions<CarDealerDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PartCar>()
              .HasKey(pc => new { pc.PartId, pc.CarId });

            modelBuilder.Entity<PartCar>()
                .HasOne(pc => pc.Part)
                .WithMany(p => p.Cars)
                .HasForeignKey(pc => pc.PartId);

            modelBuilder.Entity<PartCar>()
                .HasOne(pc => pc.Car)
                .WithMany(c => c.Parts)
                .HasForeignKey(pc => pc.CarId);

            modelBuilder.Entity<Sale>()
                .HasOne(s => s.Customer)
                .WithMany(c => c.Sales)
                .HasForeignKey(s => s.CustomerId);

            modelBuilder.Entity<Sale>()
                .HasOne(s => s.Car)
                .WithMany(c => c.Sales)
                .HasForeignKey(s => s.CarId);

            modelBuilder.Entity<Part>()
                .HasOne(p => p.Supplier)
                .WithMany(s => s.Parts)
                .HasForeignKey(p => p.SupplierId);

            modelBuilder.Entity<Log>()
                .HasOne(l => l.User)
                .WithMany(u => u.Logs)
                .HasForeignKey(l => l.UserId);

            base.OnModelCreating(modelBuilder);
        }
    }
}