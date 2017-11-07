namespace _03.FootballBetting.Client
{
    using _03.FootballBetting.Data;

    public class StartUp
    {
        public static void Main()
        {
            using (FootballBettingDbContext database = new FootballBettingDbContext())
            {
                PrepareDatabase(database);
            }
        }

        private static void PrepareDatabase(FootballBettingDbContext database)
        {
            database.Database.EnsureDeleted();
            database.Database.EnsureCreated();
        }
    }
}