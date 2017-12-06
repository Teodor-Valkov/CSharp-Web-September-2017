namespace FitStore.Services.Contracts
{
    using Models.Subcategories;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ISubcategoryService
    {
        Task<IEnumerable<SubcategoryAdvancedServiceModel>> GetAllListingAsync(string searchToken, int page);

        Task<IEnumerable<SubcategoryBasicServiceModel>> GetAllBasicListingAsync(int categoryId);

        Task<SubcategoryDetailsServiceModel> GetDetailsByIdAsync(int categoryId);

        Task<bool> IsSubcategoryExistingById(int subcategoryId);

        Task<bool> IsSubcategoryExistingByName(string name);

        Task<int> TotalCountAsync(string searchToken);
    }
}