namespace CarDealer.Services.Contracts
{
    using Models.Suppliers;
    using System.Collections.Generic;

    public interface ISupplierService
    {
        IEnumerable<SuppliersListServiceModel> GetAllListing(int page, int pageSize);

        IEnumerable<SuppliersListServiceModel> GetAllByType(bool isImporter);

        IEnumerable<SupplierServiceModel> GetAllPartSuppliers();

        SupplierDetailsServiceModel GetSupplierById(int id);

        void Create(string name, bool isImporter);

        void Edit(int id, string name, bool isImporter);

        void Delete(int id);

        int TotalSuppliersCount();
    }
}