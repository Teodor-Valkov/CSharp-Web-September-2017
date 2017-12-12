namespace FitStore.Services.Contracts
{
    using Models.Categories;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ICategoryService
    {
        Task<IEnumerable<CategoryAdvancedServiceModel>> GetAllAdvancedListingAsync();

        Task<IEnumerable<CategoryBasicServiceModel>> GetAllBasicListingAsync();

        Task<CategoryDetailsServiceModel> GetDetailsByIdAsync(int categoryId);

        Task<bool> IsCategoryExistingById(int categoryId, bool isDeleted);

        Task<bool> IsCategoryExistingById(int categoryId);

        Task<bool> IsCategoryExistingByName(string name);

        Task<bool> IsCategoryExistingByIdAndName(int categoryId, string name);
    }
}