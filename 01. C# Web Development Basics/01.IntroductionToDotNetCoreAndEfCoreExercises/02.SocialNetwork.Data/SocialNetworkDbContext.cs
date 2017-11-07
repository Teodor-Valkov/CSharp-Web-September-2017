namespace _02.SocialNetwork.Data
{
    using _02.SocialNetwork.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class SocialNetworkDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Friendship> Friendships { get; set; }

        public DbSet<Album> Albums { get; set; }

        public DbSet<Picture> Pictures { get; set; }

        public DbSet<Tag> Tags { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=.;Database=SocialNetworkDb;Integrated Security=true;");

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Friendship>()
               .HasKey(f => new { f.FromUserId, f.ToUserId });

            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.FromUser)
                .WithMany(u => u.FromFriends)
                .HasForeignKey(f => f.FromUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.ToUser)
                .WithMany(u => u.ToFriends)
                .HasForeignKey(f => f.ToUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AlbumPicture>()
               .HasKey(ap => new { ap.AlbumId, ap.PictureId });

            modelBuilder.Entity<AlbumPicture>()
                .HasOne(ap => ap.Album)
                .WithMany(a => a.Pictures)
                .HasForeignKey(ap => ap.AlbumId);

            modelBuilder.Entity<AlbumPicture>()
                .HasOne(ap => ap.Picture)
                .WithMany(p => p.Albums)
                .HasForeignKey(ap => ap.PictureId);

            modelBuilder.Entity<Album>()
                .HasOne(a => a.Creator)
                .WithMany(u => u.Albums)
                .HasForeignKey(a => a.CreatorId);

            modelBuilder.Entity<AlbumTag>()
               .HasKey(at => new { at.AlbumId, at.TagId });

            modelBuilder.Entity<AlbumTag>()
                .HasOne(at => at.Album)
                .WithMany(a => a.Tags)
                .HasForeignKey(at => at.AlbumId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AlbumTag>()
                .HasOne(at => at.Tag)
                .WithMany(t => t.Albums)
                .HasForeignKey(at => at.TagId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AlbumUser>()
            .HasKey(au => new { au.AlbumId, au.UserId });

            modelBuilder.Entity<AlbumUser>()
                .HasOne(au => au.Album)
                .WithMany(a => a.SharedAlbums)
                .HasForeignKey(au => au.AlbumId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AlbumUser>()
                .HasOne(au => au.User)
                .WithMany(u => u.SharedAlbums)
                .HasForeignKey(au => au.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            IServiceProvider serviceProvider = this.GetService<IServiceProvider>();
            IDictionary<object, object> items = new Dictionary<object, object>();

            foreach (EntityEntry entry in this.ChangeTracker.Entries().Where(e => (e.State == EntityState.Added) || (e.State == EntityState.Modified)))
            {
                object entity = entry.Entity;
                ValidationContext context = new ValidationContext(entity, serviceProvider, items);
                IList<ValidationResult> results = new List<ValidationResult>();

                if (Validator.TryValidateObject(entity, context, results, true) == false)
                {
                    foreach (ValidationResult result in results)
                    {
                        if (result != ValidationResult.Success)
                        {
                            throw new ValidationException(result.ErrorMessage);
                        }
                    }
                }
            }

            return base.SaveChanges(acceptAllChangesOnSuccess);
        }
    }
}