namespace FitStore.Services.Manager.Contracts
{
    using Models.Categories;
    using System.Threading.Tasks;

    public interface IManagerCategoryService
    {
        Task CreateAsync(string name);

        Task<CategoryBasicServiceModel> GetEditModelAsync(int categoryId);

        Task EditAsync(int categoryId, string name);

        Task DeleteAsync(int categoryId);

        Task RestoreAsync(int categoryId);

        Task<bool> IsCategoryExistingById(int categoryId);

        Task<bool> IsCategoryExistingByName(string name);
    }
}