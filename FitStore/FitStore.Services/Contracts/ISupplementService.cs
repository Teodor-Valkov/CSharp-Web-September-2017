namespace FitStore.Services.Contracts
{
    using Models.Supplements;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    public interface ISupplementService
    {
        Task<IEnumerable<SupplementAdvancedServiceModel>> GetAllAdvancedListingAsync(string searchToken, int page);

        Task<SupplementDetailsServiceModel> GetDetailsByIdAsync(int supplementId, int page);

        Task<bool> IsSupplementExistingById(int supplementId, bool isDeleted);

        Task<bool> IsSupplementExistingByName(string name);

        Task<bool> IsSupplementExistingByIdAndName(int supplementId, string name);

        Task<int> TotalCommentsAsync(int supplementId, bool shouldSeeDeletedComments);

        Task<int> TotalCountAsync(string searchToken);
    }
}