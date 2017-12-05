namespace BookShop.Services.Contracts
{
    using Models.Categories;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDetailsServiceModel>> GetAllListingAsync();

        Task<CategoryDetailsServiceModel> GetCategoryDetailsByIdAsync(int categoryId);

        Task<int> CreateAsync(string name);

        Task<int> EditAsync(int categoryId, string name);

        Task<int> DeleteAsync(int categoryId);

        Task<bool> IsCategoryExisting(int categoryId);
    }
}