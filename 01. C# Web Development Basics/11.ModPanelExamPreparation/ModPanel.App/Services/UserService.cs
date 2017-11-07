namespace ModPanel.App.Services
{
    using AutoMapper.QueryableExtensions;
    using Data;
    using Data.Models;
    using Data.Models.Enums;
    using Models.Admin;
    using Services.Contracts;
    using System.Collections.Generic;
    using System.Linq;

    public class UserService : IUserService
    {
        private readonly ModPanelDbContext database;

        public UserService(ModPanelDbContext database)
        {
            this.database = database;
        }

        public bool Create(string email, string password, PositionType position)
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
                IsAdmin = isAdmin,
                Position = position,
                IsApproved = isAdmin
            };

            this.database.Add(user);
            this.database.SaveChanges();

            return true;
        }

        public bool UserExists(string email, string password)
        {
            return this.database.Users.Any(u => u.Email == email && u.Password == password);
        }

        public bool UserIsApproved(string email)
        {
            return this.database.Users.Any(u => u.Email == email && u.IsApproved);
        }

        public string Approve(int id)
        {
            User user = this.database.Users.Find(id);

            if (user != null)
            {
                user.IsApproved = true;

                this.database.SaveChanges();
            }

            return user?.Email;
        }

        public IEnumerable<AdminUsersListModel> All()
        {
            return this.database
                   .Users
                   .ProjectTo<AdminUsersListModel>()
                   .ToList();
        }
    }
}