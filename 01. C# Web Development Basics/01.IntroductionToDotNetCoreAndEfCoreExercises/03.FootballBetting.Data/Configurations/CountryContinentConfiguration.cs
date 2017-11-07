namespace _03.FootballBetting.Data.Configurations
{
    using _03.FootballBetting.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class CountryContinentConfiguration : IEntityTypeConfiguration<CountryContinent>
    {
        public void Configure(EntityTypeBuilder<CountryContinent> builder)
        {
            builder
                .HasKey(cc => new { cc.ContinentId, cc.CountryId });

            builder
                .HasOne(cc => cc.Continent)
                .WithMany(c => c.Countries)
                .HasForeignKey(cc => cc.ContinentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(cc => cc.Country)
                .WithMany(c => c.Continents)
                .HasForeignKey(cc => cc.CountryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}