namespace FitStore.Services.Implementations
{
    using AutoMapper.QueryableExtensions;
    using Contracts;
    using Data;
    using Data.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Models.Users;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class UserService : IUserService
    {
        private readonly FitStoreDbContext database;
        private readonly UserManager<User> userManager;

        public UserService(FitStoreDbContext database, UserManager<User> userManager)
        {
            this.database = database;
            this.userManager = userManager;
        }

        public async Task<UserProfileServiceModel> GetProfileByUsernameAsync(string username, int page)
        {
            return await this.database
              .Users
              .Where(u => u.UserName == username)
              .ProjectTo<UserProfileServiceModel>(new { username, page })
              .FirstOrDefaultAsync();
        }

        public async Task<UserEditProfileServiceModel> GetEditProfileByUsernameAsync(string username)
        {
            return await this.database
                 .Users
                 .Where(u => u.UserName == username)
                 .ProjectTo<UserEditProfileServiceModel>()
                 .FirstOrDefaultAsync();
        }

        public async Task<UserChangePasswordServiceModel> GetChangePasswordByUsernameAsync(string username)
        {
            return await this.database
               .Users
               .Where(u => u.UserName == username)
               .ProjectTo<UserChangePasswordServiceModel>()
               .FirstOrDefaultAsync();
        }

        public async Task<bool> EditProfileAsync(User user, string fullName, string email, string address, string phoneNumber, DateTime birthDate)
        {
            if (user.FullName != fullName)
            {
                user.FullName = fullName;
            }

            if (user.Email != email)
            {
                await this.userManager.SetEmailAsync(user, email);
            }

            if (user.Address != address)
            {
                user.Address = address;
            }

            if (user.PhoneNumber != phoneNumber)
            {
                await this.userManager.SetPhoneNumberAsync(user, phoneNumber);
            }

            if (user.BirthDate != birthDate)
            {
                user.BirthDate = birthDate;
            }

            IdentityResult editProfileResult = await this.userManager.UpdateAsync(user);

            return editProfileResult.Succeeded;
        }

        public async Task<bool> ChangePasswordAsync(User user, string oldPassword, string newPassword)
        {
            IdentityResult changePasswordResult = await this.userManager.ChangePasswordAsync(user, oldPassword, newPassword);

            return changePasswordResult.Succeeded;
        }

        public async Task<bool> IsUserRestricted(string username)
        {
            User user = await this.userManager.FindByNameAsync(username);

            return user.IsRestricted;
        }

        public async Task<int> TotalOrdersAsync(string username)
        {
            return await this.database
                .Users
                .Where(u => u.UserName == username)
                .SelectMany(u => u.Orders)
                .CountAsync();
        }
    }
}