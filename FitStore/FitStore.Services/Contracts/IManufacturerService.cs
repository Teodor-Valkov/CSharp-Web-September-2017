namespace FitStore.Services.Contracts
{
    using Models.Manufacturers;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IManufacturerService
    {
        Task<IEnumerable<ManufacturerAdvancedServiceModel>> GetAllListingAsync(string searchToken, int page);

        Task<ManufacturerDetailsServiceModel> GetDetailsByIdAsync(int manufacturerId);

        Task<int> TotalCountAsync(string searchToken);
    }
}