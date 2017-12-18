namespace FitStore.Services.Manager.Contracts
{
    using Models.Supplements;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IManagerSupplementService
    {
        Task<IEnumerable<SupplementAdvancedServiceModel>> GetAllPagedListingAsync(bool isDeleted, string searchToken, int page);

        Task CreateAsync(string name, string description, int quantity, decimal price, byte[] picture, DateTime bestBeforeDate, int subcategoryId, int manufacturerId);

        Task<SupplementServiceModel> GetEditModelAsync(int supplementId);

        Task EditAsync(int supplementId, string name, string description, int quantity, decimal price, byte[] picture, DateTime bestBeforeDate, int subcategoryId, int manufacturerId);

        Task DeleteAsync(int supplementId);

        Task<string> RestoreAsync(int supplementId);

        Task<bool> IsSupplementModified(int supplemetId, string name, string description, int quantity, decimal price, byte[] picture, DateTime bestBeforeDate, int subcategoryId, int manufacturerId);

        Task<int> TotalCountAsync(bool isDeleted, string searchToken);
    }
}