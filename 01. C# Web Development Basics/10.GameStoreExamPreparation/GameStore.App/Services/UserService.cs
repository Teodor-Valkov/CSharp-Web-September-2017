namespace GameStore.App.Services
{
    using Data;
    using GameStore.Models;
    using Services.Contracts;
    using System.Linq;

    public class UserService : IUserService
    {
        private readonly GameStoreDbContext database;

        public UserService(GameStoreDbContext database)
        {
            this.database = database;
        }

        public bool RegisterUser(string fullName, string email, string password)
        {
            if (this.database.Users.Any(u => u.Email == email))
            {
                return false;
            }

            bool isAdmin = !this.database.Users.Any();

            User user = new User
            {
                FullName = fullName,
                Email = email,
                Password = password,
                IsAdmin = isAdmin
            };

            this.database.Users.Add(user);
            this.database.SaveChanges();

            return true;
        }

        public bool IsUserExisting(string email, string password)
        {
            return this.database.Users.Any(u => u.Email == email && u.Password == password);
        }

        public int? GetUserId(string email)
        {
            int id = this.database
                .Users
                .Where(u => u.Email == email)
                .Select(u => u.Id)
                .FirstOrDefault();

            return id != 0 ? (int?)id : null;
        }
    }
}