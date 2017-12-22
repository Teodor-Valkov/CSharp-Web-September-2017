namespace FitStore.Services.Contracts
{
    using Data.Models;
    using Models.Users;
    using System;
    using System.Threading.Tasks;

    public interface IUserService
    {
        Task<UserProfileServiceModel> GetProfileByUsernameAsync(string username, int page);

        Task<UserEditProfileServiceModel> GetEditProfileByUsernameAsync(string username);

        Task<UserChangePasswordServiceModel> GetChangePasswordByUsernameAsync(string username);

        Task<bool> EditProfileAsync(User user, string fullName, string email, string address, string phoneNumber, DateTime birthDate);

        Task<bool> ChangePasswordAsync(User user, string oldPassword, string newPassword);

        Task<bool> IsOldPasswordValid(User user, string oldPassword);

        Task<bool> IsUserRestricted(string username);

        Task<int> TotalOrdersAsync(string username);
    }
}