namespace CarDealer.Services.Implementations
{
    using CarDealer.Data.Models;
    using Contracts;
    using Data;
    using Models.Suppliers;
    using System.Collections.Generic;
    using System.Linq;

    public class SupplierService : ISupplierService
    {
        private readonly CarDealerDbContext database;

        public SupplierService(CarDealerDbContext database)
        {
            this.database = database;
        }

        public IEnumerable<SuppliersListServiceModel> GetAllListing(int page = 1, int pageSize = 10)
        {
            return this.database
               .Suppliers
               .OrderBy(s => s.Name)
               .Skip((page - 1) * pageSize)
               .Take(pageSize)
               .Select(s => new SuppliersListServiceModel
               {
                   Id = s.Id,
                   Name = s.Name,
                   IsImporter = s.IsImporter,
                   TotalPartsCount = s.Parts.Count
               })
               .ToList();
        }

        public IEnumerable<SuppliersListServiceModel> GetAllByType(bool isImporter)
        {
            return this.database
                .Suppliers
                .OrderByDescending(s => s.Id)
                .Where(s => s.IsImporter == isImporter)
                .Select(s => new SuppliersListServiceModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    IsImporter = s.IsImporter,
                    TotalPartsCount = s.Parts.Count
                })
                .ToList();
        }

        public IEnumerable<SupplierServiceModel> GetAllPartSuppliers()
        {
            return this.database
              .Suppliers
              .OrderBy(s => s.Name)
              .Select(s => new SupplierServiceModel
              {
                  Id = s.Id,
                  Name = s.Name
              })
              .ToList();
        }

        public SupplierDetailsServiceModel GetSupplierById(int id)
        {
            return this.database
                .Suppliers
                .Where(s => s.Id == id)
                .Select(s => new SupplierDetailsServiceModel
                {
                    Name = s.Name,
                    IsImporter = s.IsImporter
                })
                .FirstOrDefault();
        }

        public void Create(string name, bool isImporter)
        {
            Supplier supplier = new Supplier
            {
                Name = name,
                IsImporter = isImporter
            };

            this.database.Suppliers.Add(supplier);
            this.database.SaveChanges();
        }

        public void Edit(int id, string name, bool isImporter)
        {
            Supplier supplier = this.database.Suppliers.Find(id);

            if (supplier == null)
            {
                return;
            }

            supplier.Name = name;
            supplier.IsImporter = isImporter;

            this.database.SaveChanges();
        }

        public void Delete(int id)
        {
            Supplier supplier = this.database.Suppliers.Find(id);

            if (supplier == null)
            {
                return;
            }

            this.database.Suppliers.Remove(supplier);
            this.database.SaveChanges();
        }

        public int TotalSuppliersCount()
        {
            return this.database.Suppliers.Count();
        }
    }
}