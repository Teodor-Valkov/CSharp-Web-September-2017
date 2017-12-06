namespace FitStore.Services.Contracts
{
    using Models.Supplements;
    using System.Threading.Tasks;

    public interface ISupplementService
    {
        Task<SupplementDetailsServiceModel> GetDetailsByIdAsync(int supplementId);

        Task<bool> IsSupplementExistingById(int supplementId);

        Task<bool> IsSupplementExistingByName(string name);
    }
}