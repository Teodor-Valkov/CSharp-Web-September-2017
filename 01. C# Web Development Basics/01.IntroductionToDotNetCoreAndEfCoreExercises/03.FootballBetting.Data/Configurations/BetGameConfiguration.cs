namespace _03.FootballBetting.Data.Configurations
{
    using _03.FootballBetting.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class BetGameConfiguration : IEntityTypeConfiguration<BetGame>
    {
        public void Configure(EntityTypeBuilder<BetGame> builder)
        {
            builder
                .HasKey(bg => new { bg.BetId, bg.GameId });

            builder
                .HasOne(bg => bg.Bet)
                .WithMany(b => b.Games)
                .HasForeignKey(bg => bg.BetId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(bg => bg.Game)
                .WithMany(g => g.Bets)
                .HasForeignKey(bg => bg.GameId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}