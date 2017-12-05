namespace CarDealer.Services.Contracts
{
    using Models.Parts;
    using System.Collections.Generic;

    public interface IPartService
    {
        IEnumerable<PartListServiceModel> GetAllListing(int page = 1, int pageSize = 10);

        IEnumerable<PartServiceModel> GetAllParts();

        PartDetailsServiceModel GetPartById(int id);

        int TotalPartsCount();

        void Create(string name, int quantity, decimal price, int supplierId);

        void Edit(int id, decimal price, int quantity);

        void Delete(int id);
    }
}