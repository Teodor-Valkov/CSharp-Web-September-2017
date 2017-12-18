namespace FitStore.Services.Contracts
{
    using Models.Categories;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ICategoryService
    {
        Task<IEnumerable<CategoryAdvancedServiceModel>> GetAllAdvancedListingAsync();

        Task<CategoryDetailsServiceModel> GetDetailsByIdAsync(int categoryId, int page);

        Task<bool> IsCategoryExistingById(int categoryId, bool isDeleted);

        Task<bool> IsCategoryExistingByName(string name);

        Task<bool> IsCategoryExistingByIdAndName(int categoryId, string name);

        Task<int> TotalSupplementsCountAsync(int categoryId);
    }
}