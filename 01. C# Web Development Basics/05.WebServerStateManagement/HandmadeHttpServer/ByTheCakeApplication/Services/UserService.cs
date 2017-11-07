namespace HandmadeHttpServer.ByTheCakeApplication.Services
{
    using HandmadeHttpServer.ByTheCakeApplication.Data;
    using HandmadeHttpServer.ByTheCakeApplication.Data.Models;
    using HandmadeHttpServer.ByTheCakeApplication.Services.Contracts;
    using HandmadeHttpServer.ByTheCakeApplication.ViewModels.Account;
    using System;
    using System.Linq;

    public class UserService : IUserService
    {
        public bool CreateUser(string fullName, string username, string password)
        {
            using (ByTheCakeDbContext database = new ByTheCakeDbContext())
            {
                if (database.Users.Any(u => u.Username == username))
                {
                    return false;
                }

                User user = new User
                {
                    FullName = fullName,
                    Username = username,
                    Password = password,
                    RegistrationDate = DateTime.UtcNow
                };

                database.Users.Add(user);
                database.SaveChanges();

                return true;
            }
        }

        public bool FindUser(string username, string password)
        {
            using (ByTheCakeDbContext database = new ByTheCakeDbContext())
            {
                return database.Users.Any(u => u.Username == username && u.Password == password);
            }
        }

        public ProfileViewModel GetProfile(string username)
        {
            using (ByTheCakeDbContext database = new ByTheCakeDbContext())
            {
                return database.Users
                    .Where(u => u.Username == username)
                    .Select(u => new ProfileViewModel
                    {
                        FullName = u.FullName,
                        Username = u.Username,
                        RegistrationDate = u.RegistrationDate,
                        TotalOrders = u.Orders.Count()
                    })
                    .FirstOrDefault();
            }
        }

        public int? GetUserId(string username)
        {
            using (ByTheCakeDbContext database = new ByTheCakeDbContext())
            {
                int id = database.Users
                    .Where(u => u.Username == username)
                    .Select(u => u.Id)
                    .FirstOrDefault();

                return id != 0 ? (int?)id : null;
            }
        }
    }
}