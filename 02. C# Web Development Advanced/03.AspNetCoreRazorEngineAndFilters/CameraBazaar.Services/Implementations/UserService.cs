namespace CameraBazaar.Services.Implementations
{
    using Data;
    using Data.Common;
    using Data.Models;
    using Microsoft.AspNetCore.Identity;
    using Models.Cameras;
    using Services.Contracts;
    using Services.Models.Users;
    using System.Linq;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System;

    public class UserService : IUserService
    {
        private CameraBazaarDbContext database;
        private UserManager<User> userManager;
        private ICameraService cameraService;

        public UserService(CameraBazaarDbContext database, UserManager<User> userManager, ICameraService cameraService)
        {
            this.database = database;
            this.userManager = userManager;
            this.cameraService = cameraService;
        }

        public UserDetailsServiceModel GetUserDetailsByUsername(string username)
        {
            return this.database
                .Users
                .Where(u => u.UserName.ToLower() == username.ToLower())
                .Select(u => new UserDetailsServiceModel
                {
                    Username = u.UserName,
                    Email = u.Email,
                    Phone = u.PhoneNumber,
                    Cameras = $"{string.Format(MessageConstants.CamerasMessage, u.Cameras.Where(c => c.Quantity != 0).Count(), u.Cameras.Where(c => c.Quantity == 0).Count())}",
                    CamerasOwned = this.GetAllCamerasByUsername(username),
                    LastLoginTime = u.LastLoginTime
                })
                .FirstOrDefault();
        }

        public UserEditServiceModel GetUserEditDetailsByUsername(string username)
        {
            return this.database
                .Users
                .Where(u => u.UserName.ToLower() == username.ToLower())
                .Select(u => new UserEditServiceModel
                {
                    Email = u.Email,
                    Phone = u.PhoneNumber,
                    LastLoginTime = u.LastLoginTime
                })
                .FirstOrDefault();
        }

        public async Task<ICollection<IdentityError>> Edit(string username, string email, string oldPassword, string newPassword, string phone)
        {
            User user = await this.userManager.FindByNameAsync(username);

            if (user == null)
            {
                return null;
            }

            ICollection<IdentityError> errors = new List<IdentityError>();

            string emailToken = await this.userManager.GenerateChangeEmailTokenAsync(user, email);
            IdentityResult emailResult = await this.userManager.ChangeEmailAsync(user, email, emailToken);

            if (!emailResult.Succeeded)
            {
                AddErrors(errors, emailResult);
            }

            if (!string.IsNullOrEmpty(oldPassword) && !string.IsNullOrEmpty(newPassword))
            {
                string passwordToken = await this.userManager.GeneratePasswordResetTokenAsync(user);
                IdentityResult passwordResult = await this.userManager.ChangePasswordAsync(user, oldPassword, newPassword);

                if (!passwordResult.Succeeded)
                {
                    AddErrors(errors, passwordResult);
                }
            }

            string phoneToken = await this.userManager.GenerateChangePhoneNumberTokenAsync(user, phone);
            IdentityResult phoneResult = await this.userManager.ChangePhoneNumberAsync(user, phone, phoneToken);

            if (!phoneResult.Succeeded)
            {
                AddErrors(errors, phoneResult);
            }

            return errors;
        }

        private IEnumerable<CameraListServiceModel> GetAllCamerasByUsername(string username)
        {
            return this.database
                .Cameras
                .Where(c => c.User.UserName.ToLower() == username.ToLower())
                .Select(c => new CameraListServiceModel
                {
                    Id = c.Id,
                    Make = c.Make,
                    Model = c.Model,
                    Price = c.Price,
                    Status = c.Quantity != 0 ? MessageConstants.InStockMessage : MessageConstants.OutOfStockMessage,
                    ImageUrl = c.ImageUrl
                })
                .ToList();
        }

        public void UpdateLastTimeLogin(string username)
        {
            User user = this.database.Users.FirstOrDefault(u => u.UserName.ToLower() == username.ToLower());

            if (user == null)
            {
                return;
            }

            user.LastLoginTime = DateTime.UtcNow;
            database.SaveChanges();
        }

        private static void AddErrors(ICollection<IdentityError> errors, IdentityResult identityResult)
        {
            foreach (IdentityError error in identityResult.Errors)
            {
                if (!errors.Any(e => e.Code == error.Code && e.Description == error.Description))
                {
                    errors.Add(error);
                }
            }
        }

        public bool IsUserAllowedToCreateCameras(string userId)
        {
            User user = this.database.Users.Find(userId);

            if (user == null)
            {
                return false;
            }

            return user.CanCreateCameras;
        }
    }
}