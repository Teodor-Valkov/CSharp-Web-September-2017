namespace FitStore.Services.Contracts
{
    using Models.Supplements;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    public interface ISupplementService
    {
        Task<IEnumerable<SupplementAdvancedServiceModel>> GetAllAdvancedListingAsync();

        Task<SupplementDetailsServiceModel> GetDetailsByIdAsync(int supplementId);

        Task<bool> IsSupplementExistingById(int supplementId);

        Task<bool> IsSupplementExistingByName(string name);
    }
}