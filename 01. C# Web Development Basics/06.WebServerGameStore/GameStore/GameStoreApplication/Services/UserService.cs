namespace GameStore.GameStoreApplication.Services
{
    using GameStore.GameStoreApplication.Data;
    using GameStore.GameStoreApplication.Data.Models;
    using GameStore.GameStoreApplication.Services.Contracts;
    using System.Linq;

    public class UserService : IUserService
    {
        public bool Create(string email, string name, string password)
        {
            using (GameStoreDbContext database = new GameStoreDbContext())
            {
                if (database.Users.Any(u => u.Email == email))
                {
                    return false;
                }

                bool isAdmin = !database.Users.Any();

                User user = new User
                {
                    Email = email,
                    Name = name,
                    Password = password,
                    IsAdmin = isAdmin
                };

                database.Users.Add(user);
                database.SaveChanges();
            }

            return true;
        }

        public bool Find(string email, string password)
        {
            using (GameStoreDbContext database = new GameStoreDbContext())
            {
                return database.Users.Any(u => u.Email == email && u.Password == password);
            }
        }

        public int? GetUserId(string email)
        {
            using (GameStoreDbContext database = new GameStoreDbContext())
            {
                int id = database.Users
                    .Where(u => u.Email == email)
                    .Select(u => u.Id)
                    .FirstOrDefault();

                return id != 0 ? (int?)id : null;
            }
        }

        public bool IsAdmin(string email)
        {
            using (GameStoreDbContext database = new GameStoreDbContext())
            {
                return database.Users.Any(u => u.Email == email && u.IsAdmin);
            }
        }
    }
}