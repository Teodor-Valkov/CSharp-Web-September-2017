namespace Judge.App.Services
{
    using Data;
    using Data.Models;
    using Services.Contracts;
    using System.Linq;

    public class UserService : IUserService
    {
        private readonly JudgeDbFinalExam database;

        public UserService(JudgeDbFinalExam database)
        {
            this.database = database;
        }

        public bool Create(string email, string password, string fullNme)
        {
            if (this.database.Users.Any(u => u.Email == email))
            {
                return false;
            }

            bool isAdmin = !this.database.Users.Any();

            User user = new User
            {
                Email = email,
                Password = password,
                FullName = fullNme,
                IsAdmin = isAdmin,
            };

            this.database.Add(user);
            this.database.SaveChanges();

            return true;
        }

        public bool UserExists(string email, string password)
        {
            return this.database.Users.Any(u => u.Email == email && u.Password == password);
        }
    }
}