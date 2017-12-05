namespace CameraBazaar.Services.Implementations
{
    using Contracts;
    using Data;
    using Data.Models;
    using Microsoft.AspNetCore.Identity;
    using Models.Identity;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class IdentityService : IIdentityService
    {
        private readonly UserManager<User> userManager;
        private readonly CameraBazaarDbContext database;

        public IdentityService(UserManager<User> userManager, CameraBazaarDbContext database)
        {
            this.userManager = userManager;
            this.database = database;
        }

        public IEnumerable<IdentityBasicServiceModel> GetAllListing(int page, int pageSize)
        {
            return this.database
                .Users
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new IdentityBasicServiceModel
                {
                    Id = u.Id,
                    Username = u.UserName,
                    Email = u.Email,
                    CanUserCreateCameras = u.CanCreateCameras
                })
                .ToList();
        }

        public IdentityWithRolesBasicServiceModel GetUserWithRolesById(string id)
        {
            return this.database
                .Users
                .Where(u => u.Id == id)
                .Select(u => new IdentityWithRolesBasicServiceModel
                {
                    Id = u.Id,
                    Username = u.UserName
                })
                .FirstOrDefault();
        }

        public IdentityEditServiceModel GetUserEditDetailsById(string id)
        {
            return this.database
               .Users
               .Where(u => u.Id == id)
               .Select(u => new IdentityEditServiceModel
               {
                   Username = u.UserName,
                   Email = u.Email,
                   Phone = u.PhoneNumber
               })
               .FirstOrDefault();
        }

        public IdentityBasicServiceModel GetUserDeleteDetailsById(string id)
        {
            return this.database
                .Users
                .Where(u => u.Id == id)
                .Select(u => new IdentityBasicServiceModel
                {
                    Id = u.Id,
                    Username = u.UserName,
                    Email = u.Email
                })
                .FirstOrDefault();
        }

        public async Task<bool> Create(string username, string email, string password, string phone)
        {
            User user = new User
            {
                UserName = username,
                Email = email,
                PhoneNumber = phone,
                CanCreateCameras = true
            };

            ICollection<IdentityError> errors = new List<IdentityError>();

            IdentityResult createResult = await this.userManager.CreateAsync(user, password);

            return createResult.Succeeded;
        }

        public async Task<ICollection<IdentityError>> Edit(string id, string email, string password, string phone)
        {
            User user = await this.userManager.FindByIdAsync(id);

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

            if (!string.IsNullOrEmpty(password))
            {
                string passwordToken = await this.userManager.GeneratePasswordResetTokenAsync(user);
                IdentityResult passwordResult = await this.userManager.ResetPasswordAsync(user, passwordToken, password);

                if (!passwordResult.Succeeded)
                {
                    AddErrors(errors, emailResult);
                }
            }

            string phoneToken = await this.userManager.GenerateChangePhoneNumberTokenAsync(user, phone);
            IdentityResult phoneResult = await this.userManager.ChangePhoneNumberAsync(user, phone, phoneToken);

            if (!phoneResult.Succeeded)
            {
                AddErrors(errors, emailResult);
            }

            return errors;
        }

        public async Task<bool> Delete(string id)
        {
            User user = await this.userManager.FindByIdAsync(id);

            if (user == null)
            {
                return false;
            }

            IEnumerable<Camera> cameras = this.database.Cameras.Where(c => c.UserId == user.Id).ToList();

            foreach (Camera camera in user.Cameras)
            {
                this.database.Cameras.Remove(camera);
            }

            IdentityResult deleteResult = await this.userManager.DeleteAsync(user);

            return deleteResult.Succeeded;
        }

        public void ChangeUserPermission(string id)
        {
            User user = this.database.Users.Find(id);

            if (user == null)
            {
                return;
            }

            user.CanCreateCameras = !user.CanCreateCameras;
            this.database.SaveChanges();
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
    }
}