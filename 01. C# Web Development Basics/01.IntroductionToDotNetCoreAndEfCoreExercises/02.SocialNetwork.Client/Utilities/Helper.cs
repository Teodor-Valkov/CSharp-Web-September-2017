namespace _02.SocialNetwork.Client.Utilities
{
    using _02.SocialNetwork.Data;
    using _02.SocialNetwork.Models.Enums;
    using System;
    using System.Linq;

    public static class Helper
    {
        public static void PrintLine()
        {
            Console.WriteLine(new string('-', 25));
        }

        public static void PrintUsersWithFriends(SocialNetworkDbContext database)
        {
            var filteredUsers = database.Users
                .Select(u => new
                {
                    Name = u.Username,
                    FriendsCount = u.FromFriends.Count + u.ToFriends.Count,
                    Status = u.IsDeleted ? "Inactive" : "Active"
                })
                .OrderByDescending(u => u.FriendsCount)
                .ThenBy(u => u.Name)
                .ToList();

            foreach (var user in filteredUsers)
            {
                Console.WriteLine($"User: {user.Name}");
                Console.WriteLine($"Friends count: {user.FriendsCount}");
                Console.WriteLine($"Status: {user.Status}");

                PrintLine();
            }
        }

        public static void PrintUsersWithMoreThanFiveFriends(SocialNetworkDbContext database)
        {
            var filteredUsers = database.Users
                .Where(u => !u.IsDeleted)
                .Select(u => new
                {
                    Name = u.Username,
                    RegisteredOn = u.RegisteredOn,
                    FriendsCount = u.FromFriends.Count + u.ToFriends.Count,
                    Period = DateTime.Now.Subtract(u.RegisteredOn)
                })
                .Where(u => u.FriendsCount > 5)
                .OrderBy(u => u.RegisteredOn)
                .ThenBy(u => u.FriendsCount)
                .ToList();

            foreach (var user in filteredUsers)
            {
                Console.WriteLine($"User: {user.Name}");
                Console.WriteLine($"Friends count: {user.FriendsCount}");
                Console.WriteLine($"Registered before: {user.Period.Days} days");

                PrintLine();
            }
        }

        public static void PrintAlbumsWithTotalPictures(SocialNetworkDbContext database)
        {
            var filteredAlbums = database.Albums
                .Select(a => new
                {
                    Title = a.Name,
                    OwnerUsername = a.Creator.Username,
                    PicturesCount = a.Pictures.Count
                })
                .OrderByDescending(a => a.PicturesCount)
                .ThenBy(a => a.OwnerUsername)
                .ToList();

            foreach (var album in filteredAlbums)
            {
                Console.WriteLine($"Album: {album.Title}");
                Console.WriteLine($"Pictures count: {album.PicturesCount}");
                Console.WriteLine($"Album owner: {album.OwnerUsername}");

                PrintLine();
            }
        }

        public static void PrintPicturesInMoreThanTwoAlbums(SocialNetworkDbContext database)
        {
            var filteredPictures = database.Pictures
                .Where(p => p.Albums.Count >= 2)
                .Select(p => new
                {
                    Title = p.Title,
                    AlbumsTitles = p.Albums.Select(a => a.Album.Name).ToList(),
                    OwnersUsernames = p.Albums.Select(a => a.Album.Creator.Username).ToList()
                })
                .OrderByDescending(p => p.AlbumsTitles.Count())
                .ThenBy(p => p.Title)
                .ToList();

            foreach (var picture in filteredPictures)
            {
                Console.WriteLine($"Picture: {picture.Title}");

                for (int i = 0; i < picture.AlbumsTitles.Count(); i++)
                {
                    Console.WriteLine($"---- Picture in: {picture.AlbumsTitles[i]}");
                    Console.WriteLine($"---- Album owner: {picture.OwnersUsernames[i]}");
                }

                PrintLine();
            }
        }

        public static void PrintAlbumsByGivenUser(SocialNetworkDbContext database)
        {
            int userId = 46;
            string userUsername = database.Users.FirstOrDefault(u => u.Id == userId).Username;

            var filteredAlbums = database.Albums
                .Where(a => a.CreatorId == userId)
                .Select(a => new
                {
                    IsPublic = a.IsPublic,
                    Title = a.Name,
                    Pictures = a.Pictures.Select(p => new
                    {
                        p.Picture.Title,
                        p.Picture.Path
                    })
                })
                .OrderBy(a => a.Title)
                .ToList();

            Console.WriteLine(userUsername);

            foreach (var album in filteredAlbums)
            {
                Console.WriteLine($"-- {album.Title}");

                if (album.IsPublic)
                {
                    foreach (var picture in album.Pictures)
                    {
                        Console.WriteLine($"---- {picture.Title} - {picture.Path}");
                    }
                }
                else
                {
                    Console.WriteLine("Private content!");
                }

                PrintLine();
            }
        }

        public static void PrintAlbumsByTag(SocialNetworkDbContext database)
        {
            int tagId = 10;

            var filteredAlbums = database.Albums
                .Where(a => a.Tags.Any(t => t.TagId == tagId))
                .OrderByDescending(a => a.Tags.Count)
                .ThenBy(a => a.Name)
                .Select(a => new
                {
                    Title = a.Name,
                    OwnerUsername = a.Creator.Username
                })
                .ToList();

            foreach (var album in filteredAlbums)
            {
                Console.WriteLine($"Album: {album.Title}");
                Console.WriteLine($"Album owner: {album.OwnerUsername}");

                PrintLine();
            }
        }

        public static void PrintUsersWithMoreThanThreeAlbums(SocialNetworkDbContext database)
        {
            var filteredUsers = database.Users
                .Where(u => u.Albums.Any(a => a.Tags.Count > 3))
                .Select(u => new
                {
                    Username = u.Username,
                    Albums = u.Albums
                        .Select(a => new
                        {
                            Title = a.Name,
                            TagsNames = a.Tags.Select(t => t.Tag.Name)
                        })
                        .ToList()
                })
                .OrderByDescending(u => u.Albums.Count())
                .ThenByDescending(u => u.Albums.Sum(a => a.TagsNames.Count()))
                //.ThenByDescending(u => u.Albums.SelectMany(a => a.TagsNames).Count())
                .ThenBy(u => u.Username)
                .ToList();

            foreach (var user in filteredUsers)
            {
                Console.WriteLine($"User: {user.Username}");

                foreach (var album in user.Albums)
                {
                    Console.WriteLine($"Album: {album.Title}");
                    Console.WriteLine($"Album tags: {string.Join(", ", album.TagsNames)}");
                }

                PrintLine();
            }
        }

        public static void PrintUsersWithAlbumSharers(SocialNetworkDbContext database)
        {
            var filteredUsers = database.Users
                .Select(u => new
                {
                    Username = u.Username,
                    FriendsUsernames = u.FromFriends
                                          .Select(ff => ff.ToUser.Username)
                                          .ToList(),
                    AlbumsTitles = u.FromFriends
                                     .Where(ff => ff.ToUser.SharedAlbums.Any(sa => sa.UserId == ff.ToUserId))
                                     .Select(ff => ff.ToUser.SharedAlbums.Select(sa => sa.Album.Name))
                                     .ToList()
                })
                .OrderBy(u => u.Username)
                .ToList();

            foreach (var user in filteredUsers)
            {
                Console.WriteLine($"User: {user.Username}");

                for (int i = 0; i < user.FriendsUsernames.Count(); i++)
                {
                    Console.WriteLine($"---- Friend: {user.FriendsUsernames[i]}");
                    Console.WriteLine($"---- Shared albums: {string.Join(", ", user.AlbumsTitles[i])}");

                    PrintLine();
                }
            }
        }

        public static void PrintAllAlbumsSharedWithMoreThanTwoPeople(SocialNetworkDbContext database)
        {
            var filteredAlbums = database.Albums
                .Where(a => a.SharedAlbums.Where(sa => sa.Role == Role.Viewer).Count() > 2)
                .Select(a => new
                {
                    Title = a.Name,
                    ViewersCount = a.SharedAlbums.Where(sa => sa.Role == Role.Viewer).Count(),
                    IsPublic = a.IsPublic
                })
                .OrderByDescending(a => a.ViewersCount)
                .ThenBy(a => a.Title)
                .ToList();

            foreach (var album in filteredAlbums)
            {
                Console.WriteLine($"Album: {album.Title}");
                Console.WriteLine($"---- Shared with: {album.ViewersCount} people");
                Console.WriteLine($"---- Status: {(album.IsPublic ? "Public" : "Private")}");

                PrintLine();
            }
        }

        public static void PrintAllAlbumsSharedWithGivenUser(SocialNetworkDbContext database)
        {
            var username = "Username 1";

            var filteredAlbums = database.Albums
                .Where(sa => sa.SharedAlbums.Any(a => a.User.Username == username && a.Role == Role.Viewer))
                .Select(a => new
                {
                    Title = a.Name,
                    PicturesCount = a.Pictures.Count
                })
                .OrderByDescending(a => a.PicturesCount)
                .ThenBy(a => a.Title)
                .ToList();

            Console.WriteLine($"Albums shared with: {username}");

            foreach (var album in filteredAlbums)
            {
                Console.WriteLine($"---- Album: {album.Title}");
                Console.WriteLine($"---- Pictures count: {album.PicturesCount}");

                PrintLine();
            }
        }

        public static void PrintAllAlbumsWithTheirUsers(SocialNetworkDbContext database)
        {
            var filteredAlbums = database.Albums
                .Select(a => new
                {
                    Title = a.Name,
                    Users = a.SharedAlbums
                        .Select(sa => new
                        {
                            Username = sa.User.Username,
                            Role = sa.Role
                        })
                        .OrderBy(u => u.Role)
                })
                .ToList()
                .OrderBy(a => a.Users.FirstOrDefault().Username)
                .OrderByDescending(a => a.Users.Where(u => u.Role == Role.Viewer).Count())
                .ToList();

            foreach (var album in filteredAlbums)
            {
                Console.WriteLine($"Album: {album.Title}");

                foreach (var user in album.Users)
                {
                    Console.WriteLine($"---- User: {user.Username}");
                    Console.WriteLine($"---- Role: {user.Role}");
                }

                PrintLine();
            }
        }

        public static void PrintGivenUserWithHisOwnerAlbumsCountAndViewerAlbumsCount(SocialNetworkDbContext database)
        {
            var username = "Username 1";

            var filteredUser = database.Users
                .Where(u => u.Username == username)
                .Select(u => new
                {
                    Username = u.Username,
                    OwnerAlbumsCount = u.SharedAlbums.Where(sa => sa.Role == Role.Owner).Count(),
                    ViewerAlbumsCount = u.SharedAlbums.Where(sa => sa.Role == Role.Viewer).Count()
                })
                .FirstOrDefault();

            Console.WriteLine($"Albums of user: {filteredUser.Username}");
            Console.WriteLine($"---- Owner albums count: {filteredUser.OwnerAlbumsCount}");
            Console.WriteLine($"---- Viewer albums count: {filteredUser.ViewerAlbumsCount}");
        }

        public static void PrintAllUsersWhoAreViewersOfAtLeastOneAlbum(SocialNetworkDbContext database)
        {
            var filteredUsers = database.Users
                .Where(u => u.SharedAlbums.Any(sa => sa.Role == Role.Viewer))
                .Select(u => new
                {
                    Username = u.Username,
                    ViewersAlbumsCount = u.SharedAlbums.Where(sa => sa.Album.IsPublic && sa.Role == Role.Viewer).Count()
                })
                .ToList();

            foreach (var user in filteredUsers)
            {
                Console.WriteLine($"User: {user.Username}");
                Console.WriteLine($"Viewer public albums count: {user.ViewersAlbumsCount}");

                PrintLine();
            }
        }
    }
}