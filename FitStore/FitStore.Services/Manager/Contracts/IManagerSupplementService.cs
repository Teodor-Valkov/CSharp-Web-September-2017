namespace FitStore.Services.Manager.Contracts
{
    using Models.Supplements;
    using System;
    using System.Threading.Tasks;

    public interface IManagerSupplementService
    {
        Task CreateAsync(string name, string description, int quantity, decimal price, byte[] picture, DateTime bestBeforeDate, int subcategoryId, int manufacturerId);

        Task<SupplementServiceModel> GetEditModelAsync(int supplementId);

        Task EditAsync(int supplemetId, string name, string description, int quantity, decimal price, byte[] picture, DateTime bestBeforeDate, int subcategoryId, int manufacturerId);

        Task DeleteAsync(int supplementId);

        Task RestoreAsync(int supplementId);
    }
}