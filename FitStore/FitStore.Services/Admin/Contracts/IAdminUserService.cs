namespace FitStore.Services.Admin.Contracts
{
    using Models.Users;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IAdminUserService
    {
        Task<IEnumerable<AdminUserBasicServiceModel>> GetAllListingAsync(string searchToken, int page);

        Task<AdminUserDetailsServiceModel> GetDetailsByUsernameAsync(string username);

        Task<AdminUserOrdersServiceModel> GetOrdersByUsernameAsync(string username, int page);

        Task<string> GetUsernameByOrderIdAsync(int orderId);

        Task<int> TotalCountAsync(string searchToken);
    }
}