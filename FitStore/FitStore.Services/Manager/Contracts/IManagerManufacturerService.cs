namespace FitStore.Services.Manager.Contracts
{
    using Models.Manufacturers;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IManagerManufacturerService
    {
        Task<IEnumerable<ManufacturerAdvancedServiceModel>> GetAllPagedListingAsync(bool isDeleted, string searchToken, int page);

        Task CreateAsync(string name, string address);

        Task<ManufacturerBasicServiceModel> GetEditModelAsync(int manufacturerId);

        Task EditAsync(int manufacturerId, string name, string address);

        Task DeleteAsync(int manufacturerId);

        Task RestoreAsync(int manufacturerId);

        Task<int> TotalCountAsync(bool isDeleted, string searchToken);
    }
}