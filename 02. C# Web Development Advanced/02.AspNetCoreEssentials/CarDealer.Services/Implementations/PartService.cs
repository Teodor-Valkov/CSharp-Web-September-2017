namespace CarDealer.Services.Implementations
{
    using CarDealer.Data;
    using CarDealer.Data.Models;
    using Contracts;
    using Models.Parts;
    using System.Collections.Generic;
    using System.Linq;

    public class PartService : IPartService
    {
        private readonly CarDealerDbContext database;

        public PartService(CarDealerDbContext database)
        {
            this.database = database;
        }

        public IEnumerable<PartListServiceModel> GetAllListing(int page = 1, int pageSize = 10)
        {
            return this.database
                .Parts
                .OrderByDescending(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PartListServiceModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Quantity = p.Quantity,
                    SupplierName = p.Supplier.Name
                })
                .ToList();
        }

        public IEnumerable<PartServiceModel> GetAllParts()
        {
            return this.database
                .Parts
                .Select(p => new PartServiceModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price
                })
                .ToList();
        }

        public PartDetailsServiceModel GetPartById(int id)
        {
            return this.database
                .Parts
                .Where(p => p.Id == id)
                .Select(p => new PartDetailsServiceModel
                {
                    Name = p.Name,
                    Price = p.Price,
                    Quantity = p.Quantity
                })
                .FirstOrDefault();
        }

        public void Create(string name, int quantity, decimal price, int supplierId)
        {
            Part part = new Part
            {
                Name = name,
                Quantity = quantity > 0 ? quantity : 1,
                Price = price,
                SupplierId = supplierId
            };

            this.database.Parts.Add(part);
            this.database.SaveChanges();
        }

        public void Edit(int id, decimal price, int quantity)
        {
            Part part = this.database.Parts.Find(id);

            if (part == null)
            {
                return;
            }

            part.Price = price;
            part.Quantity = quantity;

            this.database.SaveChanges();
        }

        public void Delete(int id)
        {
            Part part = this.database.Parts.Find(id);

            if (part == null)
            {
                return;
            }

            this.database.Parts.Remove(part);
            this.database.SaveChanges();
        }

        public int TotalPartsCount()
        {
            return this.database.Parts.Count();
        }
    }
}