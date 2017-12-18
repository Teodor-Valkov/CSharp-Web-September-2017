namespace FitStore.Services.Contracts
{
    using Models.Manufacturers;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IManufacturerService
    {
        Task<IEnumerable<ManufacturerAdvancedServiceModel>> GetAllPagedListingAsync(int page);

        Task<ManufacturerDetailsServiceModel> GetDetailsByIdAsync(int manufacturerId, int page);

        Task<bool> IsManufacturerExistingById(int manufacturerId, bool isDeleted);

        Task<bool> IsManufacturerExistingByName(string name);

        Task<bool> IsManufacturerExistingByIdAndName(int manufacturerId, string name);

        Task<int> TotalCountAsync();

        Task<int> TotalSupplementsCountAsync(int manufacturerId);
    }
}