namespace FitStore.Services.Contracts
{
    using Models.Manufacturers;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IManufacturerService
    {
        Task<IEnumerable<ManufacturerBasicServiceModel>> GetAllBasicListingAsync();

        Task<ManufacturerDetailsServiceModel> GetDetailsByIdAsync(int manufacturerId);

        Task<bool> IsManufacturerExistingById(int manufacturerId);

        Task<bool> IsManufacturerExistingByName(string name);
    }
}