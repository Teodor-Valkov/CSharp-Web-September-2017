namespace FitStore.Services.Manager.Contracts
{
    using Models.Categories;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IManagerCategoryService
    {
        Task<IEnumerable<CategoryAdvancedServiceModel>> GetAllPagedListingAsync(bool isDeleted, string searchToken, int page);

        Task CreateAsync(string name);

        Task<CategoryBasicServiceModel> GetEditModelAsync(int categoryId);

        Task EditAsync(int categoryId, string name);

        Task DeleteAsync(int categoryId);

        Task RestoreAsync(int categoryId);

        Task<int> TotalCountAsync(bool isDeleted, string searchToken);
    }
}