namespace _02.SocialNetwork.Client
{
    using _02.SocialNetwork.Client.Utilities;
    using _02.SocialNetwork.Data;
    using Microsoft.EntityFrameworkCore;

    public class StartUp
    {
        public static void Main()
        {
            using (SocialNetworkDbContext database = new SocialNetworkDbContext())
            {
                // Seeding exercises are in class Seeder.cs
                // Printing exercises are in class Helper.cs

                // 0. A method for seeding users from given sql script
                //
                // In order to work the command 'database.Database.Migrate(); should be commented,
                // because the method is using database.Database.EnsureCreated(); and the two commands cannot work together
                //
                //Seeder.SeedUsersFromSqlScript(database);

                database.Database.Migrate();

                // 1. Tasks Users
                // 2. Tasks Frieds
                //
                // Seeder.SeedUsersAndFriends(database);
                //
                // Helper.PrintUsersWithFriends(database);
                // Helper.PrintUsersWithMoreThanFiveFriends(database);

                // 3. Tasks Albums
                //
                // Seeder.SeedAlbumsPicturesAndRoles(database);
                //
                // Helper.PrintAlbumsWithTotalPictures(database);
                // Helper.PrintPicturesInMoreThanTwoAlbums(database);
                // Helper.PrintAlbumsByGivenUser(database);

                // 4. Tasks Tags
                //
                // Seeder.SeedTags(database);
                // Helper.PrintAlbumsByTag(database);
                // Helper.PrintUsersWithMoreThanThreeAlbums(database);

                // 5. Tasks Shared Albums
                // Helper.PrintUsersWithAlbumSharers(database);
                // Helper.PrintAllAlbumsSharedWithMoreThanTwoPeople(database);
                // Helper.PrintAllAlbumsSharedWithGivenUser(database);

                // 6. Tasks User Roles
                // Helper.PrintAllAlbumsWithTheirUsers(database);
                // Helper.PrintGivenUserWithHisOwnerAlbumsCountAndViewerAlbumsCount(database);
                // Helper.PrintAllUsersWhoAreViewersOfAtLeastOneAlbum(database);
            }
        }
    }
}