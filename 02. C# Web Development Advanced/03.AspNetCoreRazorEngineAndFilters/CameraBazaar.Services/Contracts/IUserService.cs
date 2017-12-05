namespace CameraBazaar.Services.Contracts
{
    using Microsoft.AspNetCore.Identity;
    using Models.Users;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IUserService
    {
        UserDetailsServiceModel GetUserDetailsByUsername(string username);

        UserEditServiceModel GetUserEditDetailsByUsername(string username);

        Task<ICollection<IdentityError>> Edit(string username, string email, string oldPassword, string newPassword, string phone);

        void UpdateLastTimeLogin(string username);

        bool IsUserAllowedToCreateCameras(string userId);
    }
}