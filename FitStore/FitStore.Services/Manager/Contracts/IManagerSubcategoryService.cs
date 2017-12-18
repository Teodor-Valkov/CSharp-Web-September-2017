namespace FitStore.Services.Manager.Contracts
{
    using Models.Subcategories;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IManagerSubcategoryService
    {
        Task<IEnumerable<SubcategoryAdvancedServiceModel>> GetAllPagedListingAsync(bool isDeleted, string searchToken, int page);

        Task<IEnumerable<SubcategoryBasicServiceModel>> GetAllBasicListingAsync(int categoryId, bool isDeleted);

        Task CreateAsync(string name, int categoryId);

        Task<SubcategoryBasicServiceModel> GetEditModelAsync(int subcategoryId);

        Task EditAsync(int subcategoryId, string name, int categoryId);

        Task DeleteAsync(int subcategoryId);

        Task<string> RestoreAsync(int subcategoryId);

        Task<bool> IsSubcategoryModified(int subcategoryId, string name, int categoryId);

        Task<int> TotalCountAsync(bool isDeleted, string searchToken);
    }
}