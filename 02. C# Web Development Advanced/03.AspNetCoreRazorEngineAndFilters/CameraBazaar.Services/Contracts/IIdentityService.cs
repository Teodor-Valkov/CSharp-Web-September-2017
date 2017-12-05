namespace CameraBazaar.Services.Contracts
{
    using Microsoft.AspNetCore.Identity;
    using Services.Models.Identity;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IIdentityService
    {
        IEnumerable<IdentityBasicServiceModel> GetAllListing(int page, int pageSize);

        IdentityWithRolesBasicServiceModel GetUserWithRolesById(string id);

        IdentityEditServiceModel GetUserEditDetailsById(string id);

        IdentityBasicServiceModel GetUserDeleteDetailsById(string id);

        Task<bool> Create(string username, string email, string password, string phone);

        Task<ICollection<IdentityError>> Edit(string id, string email, string password, string phone);

        Task<bool> Delete(string id);

        void ChangeUserPermission(string id);
    }
}