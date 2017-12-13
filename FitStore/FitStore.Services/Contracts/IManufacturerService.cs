namespace FitStore.Services.Contracts
{
    using Models.Manufacturers;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IManufacturerService
    {
        Task<IEnumerable<ManufacturerAdvancedServiceModel>> GetAllPagedListingAsync(int page);

        Task<ManufacturerDetailsServiceModel> GetDetailsByIdAsync(int manufacturerId);

        Task<bool> IsManufacturerExistingById(int manufacturerId, bool isDeleted);

        Task<bool> IsManufacturerExistingById(int manufacturerId);

        Task<bool> IsManufacturerExistingByName(string name);

        Task<bool> IsManufacturerExistingByIdAndName(int manufacturerId, string name);

        Task<int> TotalCountAsync();
    }
}