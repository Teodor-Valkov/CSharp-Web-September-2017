namespace FitStore.Services.Manager.Contracts
{
    using FitStore.Services.Models.Manufacturers;
    using System.Threading.Tasks;

    public interface IManagerManufacturerService
    {
        Task CreateAsync(string name, string address);

        Task<ManufacturerBasicServiceModel> GetEditModelAsync(int manufacturerId);

        Task EditAsync(int manufacturerId, string name, string address);

        Task DeleteAsync(int manufacturerId);

        Task RestoreAsync(int manufacturerId);

        Task<bool> IsManufacturerExistingById(int manufacturerId);

        Task<bool> IsManufacturerExistingByName(string name);
    }
}