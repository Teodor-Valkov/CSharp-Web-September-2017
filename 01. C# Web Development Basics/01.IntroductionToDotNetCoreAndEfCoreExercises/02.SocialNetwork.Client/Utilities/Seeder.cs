namespace _02.SocialNetwork.Client.Utilities
{
    using _02.SocialNetwork.Data;
    using _02.SocialNetwork.Models;
    using _02.SocialNetwork.Models.Enums;
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;

    public static class Seeder
    {
        private static Random random = new Random();

        public static void SeedUsersFromSqlScript(SocialNetworkDbContext database)
        {
            // Adding users from sql script
            //
            database.Database.EnsureCreated();

            string connectionString = "Server=.;Database=SocialNetworkDb;Integrated Security=true;";
            SqlConnection connection = new SqlConnection(connectionString);

            connection.Open();

            string createTablesString = File.ReadAllText("../02.SocialNetwork.Data/InitialData.sql");
            SqlCommand createUsersTable = new SqlCommand(createTablesString, connection);
            using (connection)
            {
                createUsersTable.ExecuteNonQuery();
            }
        }

        public static void SeedUsersAndFriends(SocialNetworkDbContext database)
        {
            // Adding Users
            //
            const int totalUsers = 50;

            int biggestUserId = database.Users.Select(u => u.Id).LastOrDefault() + 1;

            List<User> addedUsers = new List<User>();

            for (int i = biggestUserId; i < biggestUserId + totalUsers; i++)
            {
                User user = new User
                {
                    Username = $"Username {i}",
                    Password = $"Password #{i}",
                    Email = $"email@email{i}.com",
                    Age = i + 1,
                    RegisteredOn = DateTime.Now.AddDays(-(100 + i * 10)),
                    LastTimeLoggedIn = DateTime.Now.AddDays(-i),
                    IsDeleted = random.Next(0, 2) == 0 ? true : false
                };

                addedUsers.Add(user);
                database.Users.Add(user);
            }

            database.SaveChanges();

            // Adding Frindships
            //
            List<int> usersIds = addedUsers.Select(u => u.Id).ToList();

            List<Friendship> addedFriendships = new List<Friendship>();

            for (int i = 0; i < usersIds.Count; i++)
            {
                int currentUserId = usersIds[i];
                int totalFriends = random.Next(3, 8);

                for (int j = 0; j < totalFriends; j++)
                {
                    int friendUserId = usersIds[random.Next(0, usersIds.Count)];

                    bool validFriendship = true;

                    // Cannot be friend to myself
                    if (currentUserId == friendUserId)
                    {
                        validFriendship = false;
                    }

                    // Cannot add already existing friendship
                    if (addedFriendships.Any(f =>
                            f.FromUserId == currentUserId && f.ToUserId == friendUserId ||
                            f.FromUserId == friendUserId && f.ToUserId == currentUserId))
                    {
                        validFriendship = false;
                    }

                    if (!validFriendship)
                    {
                        j--;
                        continue;
                    }

                    Friendship friendship = new Friendship
                    {
                        FromUserId = currentUserId,
                        ToUserId = friendUserId
                    };

                    addedFriendships.Add(friendship);
                    database.Friendships.Add(friendship);
                }
            }

            database.SaveChanges();
        }

        public static void SeedAlbumsPicturesAndRoles(SocialNetworkDbContext database)
        {
            const int totalAlbums = 50;
            const int totalPictures = 100;

            // Adding Albums
            //
            int biggestAlbumId = database.Albums.Select(a => a.Id).LastOrDefault() + 1;

            List<int> usersIds = database.Users.Select(u => u.Id).ToList();

            List<Album> addedAlbums = new List<Album>();

            for (int i = biggestAlbumId; i < biggestAlbumId + totalAlbums; i++)
            {
                Album album = new Album
                {
                    Name = $"Album {i}",
                    BackgroundColor = $"Color {i}",
                    IsPublic = random.Next(0, 2) == 0 ? true : false,
                    CreatorId = usersIds[random.Next(0, usersIds.Count)]
                };

                addedAlbums.Add(album);
                database.Albums.Add(album);
            }

            database.SaveChanges();

            // Adding Pictures
            //
            int biggestPictureId = database.Pictures.Select(a => a.Id).LastOrDefault() + 1;

            List<int> albumsIds = addedAlbums.Select(a => a.Id).ToList();

            List<Picture> addedPictures = new List<Picture>();

            for (int i = biggestPictureId; i < biggestPictureId + totalPictures; i++)
            {
                Picture picture = new Picture
                {
                    Title = $"Picture {i}",
                    Caption = $"Caption {i}",
                    Path = $"Path {i}"
                };

                addedPictures.Add(picture);
                database.Pictures.Add(picture);
            }

            database.SaveChanges();

            // Adding Albums-Pictures relations
            //
            List<int> picturesIds = addedPictures.Select(a => a.Id).ToList();

            List<AlbumPicture> addedAlbumPictures = new List<AlbumPicture>();

            for (int i = 0; i < addedPictures.Count; i++)
            {
                int pictureId = picturesIds[i];

                int albumsOfCurrentPicture = random.Next(3, 8);

                for (int j = 0; j < albumsOfCurrentPicture; j++)
                {
                    int albumId = albumsIds[random.Next(0, albumsIds.Count)];

                    // Cannot add picture in album twice
                    if (addedAlbumPictures.Any(ap => ap.PictureId == pictureId && ap.AlbumId == albumId))
                    {
                        j--;
                        continue;
                    }

                    AlbumPicture albumPicture = new AlbumPicture
                    {
                        PictureId = pictureId,
                        AlbumId = albumId
                    };

                    addedAlbumPictures.Add(albumPicture);

                    Picture currentPicture = database.Pictures.FirstOrDefault(p => p.Id == pictureId);
                    currentPicture.Albums.Add(albumPicture);
                }

                database.SaveChanges();
            }

            // Adding Albums-Users relations

            List<AlbumUser> addedAlbumUsers = new List<AlbumUser>();

            for (int i = 0; i < addedAlbums.Count; i++)
            {
                int albumId = albumsIds[i];

                int sharersOfCurrentAlbum = random.Next(3, 8);

                for (int j = 0; j < sharersOfCurrentAlbum; j++)
                {
                    int sharerId = usersIds[random.Next(0, usersIds.Count)];

                    // Cannot add user in album twice
                    if (addedAlbumUsers.Any(au => au.AlbumId == albumId && au.UserId == sharerId))
                    {
                        j--;
                        continue;
                    }

                    AlbumUser albumUser = new AlbumUser
                    {
                        AlbumId = albumId,
                        UserId = sharerId,
                        Role = Role.Viewer
                    };

                    addedAlbumUsers.Add(albumUser);

                    Album currentAlbum = database.Albums.FirstOrDefault(a => a.Id == albumId);
                    currentAlbum.SharedAlbums.Add(albumUser);
                }

                database.SaveChanges();
            }

            // Adding only one owner for each album
            //
            for (int i = 0; i < addedAlbums.Count; i++)
            {
                Album currentAlbum = database.Albums.FirstOrDefault(a => a.Id == addedAlbums[i].Id);

                currentAlbum.SharedAlbums.FirstOrDefault(sh => sh.Role != Role.Owner).Role = Role.Owner;
            }

            database.SaveChanges();
        }

        public static void SeedTags(SocialNetworkDbContext database)
        {
            // Adding Tags
            //
            int totalTags = database.Albums.Count() * 10;

            List<Tag> addedTags = new List<Tag>();

            for (int i = 0; i < totalTags; i++)
            {
                Tag tag = new Tag
                {
                    Name = TagTransformer.Transform($"tag{i}")
                };

                addedTags.Add(tag);
                database.Tags.Add(tag);
            }

            database.SaveChanges();

            // Adding Albums-Tags relations
            //
            List<int> tagsIds = addedTags.Select(t => t.Id).ToList();

            List<int> albumsIds = database.Albums.Select(a => a.Id).ToList();

            List<AlbumTag> addedAlbumTags = new List<AlbumTag>();

            for (int i = 0; i < tagsIds.Count; i++)
            {
                int tagId = tagsIds[i];

                int albumsOfCurrentTag = random.Next(3, 8);

                for (int j = 0; j < albumsOfCurrentTag; j++)
                {
                    int albumId = albumsIds[random.Next(0, albumsIds.Count)];

                    // Cannot add tag in same twice
                    if (addedAlbumTags.Any(at => at.TagId == tagId && at.AlbumId == albumId))
                    {
                        j--;
                        continue;
                    }

                    AlbumTag albumTag = new AlbumTag
                    {
                        TagId = tagId,
                        AlbumId = albumId
                    };

                    addedAlbumTags.Add(albumTag);

                    Tag currentTag = database.Tags.FirstOrDefault(t => t.Id == tagId);
                    currentTag.Albums.Add(albumTag);
                }

                database.SaveChanges();
            }
        }
    }
}