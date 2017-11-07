namespace _03.FootballBetting.Data.Configurations
{
    using _03.FootballBetting.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class PlayerStatisticsConfiguration : IEntityTypeConfiguration<PlayerStatistics>
    {
        public void Configure(EntityTypeBuilder<PlayerStatistics> builder)
        {
            builder
                .HasKey(ps => new { ps.GameId, ps.PlayerId });

            builder
                .HasOne(ps => ps.Game)
                .WithMany(g => g.Players)
                .HasForeignKey(ps => ps.GameId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(ps => ps.Player)
                .WithMany(p => p.Games)
                .HasForeignKey(ps => ps.PlayerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}