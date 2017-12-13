namespace FitStore.Services.Moderator.Contracts
{
    using Data.Models;
    using Models.Users;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IModeratorUserService
    {
        Task<IEnumerable<ModeratorUserBasicServiceModel>> GetAllListingAsync(string searchToken, int page);

        Task ChangePermission(User user);

        Task<int> TotalCountAsync(string searchToken);
    }
}