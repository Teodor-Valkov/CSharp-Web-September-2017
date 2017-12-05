namespace FitStore.Services.Contracts
{
    using Models.Categories;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ICategoryService
    {
        Task<IEnumerable<CategoryAdvancedServiceModel>> GetAllListingAsync(string searchToken, int page);

        Task<CategoryDetailsServiceModel> GetDetailsByIdAsync(int categoryId);

        Task<int> TotalCountAsync(string searchToken);
    }
}