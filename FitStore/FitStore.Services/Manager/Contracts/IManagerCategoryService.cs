namespace FitStore.Services.Manager.Contracts
{
    using Models.Categories;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IManagerCategoryService
    {
        Task<IEnumerable<CategoryAdvancedServiceModel>> GetAllPagedListingAsync(bool isDeleted, string searchToken, int page);

        Task<IEnumerable<CategoryBasicServiceModel>> GetAllBasicListingAsync(bool isDeleted);

        Task<IEnumerable<CategoryBasicServiceModel>> GetAllBasicListingWithAnySubcategoriesAsync(bool isDeleted);

        Task CreateAsync(string name);

        Task<CategoryBasicServiceModel> GetEditModelAsync(int categoryId);

        Task EditAsync(int categoryId, string name);

        Task DeleteAsync(int categoryId);

        Task RestoreAsync(int categoryId);

        Task<bool> IsCategoryModified(int categoryId, string name);

        Task<int> TotalCountAsync(bool isDeleted, string searchToken);
    }
}