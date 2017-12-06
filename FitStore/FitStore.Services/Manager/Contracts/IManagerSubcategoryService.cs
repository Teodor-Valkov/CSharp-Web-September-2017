namespace FitStore.Services.Manager.Contracts
{
    using Models.Subcategories;
    using System.Threading.Tasks;

    public interface IManagerSubcategoryService
    {
        Task CreateAsync(string name, int categoryId);

        Task<SubcategoryBasicServiceModel> GetEditModelAsync(int subcategoryId);

        Task EditAsync(int subcategoryId, string name, int categoryId);

        Task DeleteAsync(int subcategoryId);

        Task RestoreAsync(int subcategoryId);
    }
}