namespace FitStore.Data
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Models;

    public class FitStoreDbContext : IdentityDbContext<User>
    {
        public DbSet<Category> Categories { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<Manufacturer> Manufacturers { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderSupplements> OrderSupplements { get; set; }

        public DbSet<Review> Reviews { get; set; }

        public DbSet<Subcategory> Subcategories { get; set; }

        public DbSet<Supplement> Supplements { get; set; }

        public FitStoreDbContext(DbContextOptions<FitStoreDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<OrderSupplements>()
                .HasKey(os => new { os.OrderId, os.SupplementId });

            builder.Entity<OrderSupplements>()
                .HasOne(os => os.Order)
                .WithMany(o => o.Supplements)
                .HasForeignKey(os => os.OrderId);

            builder.Entity<OrderSupplements>()
                .HasOne(os => os.Supplement)
                .WithMany(s => s.Orders)
                .HasForeignKey(os => os.SupplementId);

            builder.Entity<Order>()
               .HasOne(o => o.User)
               .WithMany(u => u.Orders)
               .HasForeignKey(o => o.UserId);

            builder.Entity<Comment>()
                .HasOne(c => c.Supplement)
                .WithMany(s => s.Comments)
                .HasForeignKey(c => c.SupplementId);

            builder.Entity<Comment>()
               .HasOne(c => c.Author)
               .WithMany(a => a.Comments)
               .HasForeignKey(c => c.AuthorId);

            builder.Entity<Review>()
                .HasOne(r => r.Supplement)
                .WithMany(s => s.Reviews)
                .HasForeignKey(r => r.SupplementId);

            builder.Entity<Review>()
                .HasOne(r => r.Author)
                .WithMany(a => a.Reviews)
                .HasForeignKey(r => r.AuthorId);

            builder.Entity<Supplement>()
                .HasOne(s => s.Subcategory)
                .WithMany(s => s.Supplements)
                .HasForeignKey(s => s.SubcategoryId);

            builder.Entity<Supplement>()
                .HasOne(s => s.Manufacturer)
                .WithMany(m => m.Supplements)
                .HasForeignKey(s => s.ManufacturerId);

            builder.Entity<Subcategory>()
                .HasOne(s => s.Category)
                .WithMany(c => c.Subcategories)
                .HasForeignKey(s => s.CategoryId);

            builder.Entity<Category>()
                .HasIndex(c => c.IsDeleted);

            builder.Entity<Subcategory>()
                .HasIndex(s => s.IsDeleted);

            builder.Entity<Manufacturer>()
                .HasIndex(m => m.IsDeleted);

            builder.Entity<Supplement>()
                .HasIndex(s => s.IsDeleted);

            builder.Entity<Comment>()
                .HasIndex(c => c.IsDeleted);

            builder.Entity<Review>()
                .HasIndex(r => r.IsDeleted);

            base.OnModelCreating(builder);
        }
    }
}