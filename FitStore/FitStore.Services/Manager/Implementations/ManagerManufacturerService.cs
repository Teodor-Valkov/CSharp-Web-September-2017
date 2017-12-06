namespace FitStore.Services.Manager.Implementations
{
    using AutoMapper.QueryableExtensions;
    using Contracts;
    using Data;
    using Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Models.Manufacturers;
    using System.Linq;
    using System.Threading.Tasks;

    public class ManagerManufacturerService : IManagerManufacturerService
    {
        private readonly FitStoreDbContext database;

        public ManagerManufacturerService(FitStoreDbContext database)
        {
            this.database = database;
        }

        public async Task CreateAsync(string name, string address)
        {
            Manufacturer manufacturer = new Manufacturer
            {
                Name = name,
                Address = address
            };

            await this.database.Manufacturers.AddAsync(manufacturer);
            await this.database.SaveChangesAsync();
        }

        public async Task<ManufacturerBasicServiceModel> GetEditModelAsync(int manufacturerId)
        {
            return await this.database
             .Manufacturers
             .Where(m => m.Id == manufacturerId)
             .ProjectTo<ManufacturerBasicServiceModel>()
             .FirstOrDefaultAsync();
        }

        public async Task EditAsync(int manufacturerId, string name, string address)
        {
            Manufacturer manufacturer = await this.database
                .Manufacturers
                .Where(m => m.Id == manufacturerId)
                .FirstOrDefaultAsync();

            manufacturer.Name = name;
            manufacturer.Address = address;

            this.database.Manufacturers.Update(manufacturer);
            await this.database.SaveChangesAsync();
        }

        public async Task DeleteAsync(int manufacturerId)
        {
            Manufacturer manufacturer = await this.database
               .Manufacturers
               .Include(m => m.Supplements)
               .Where(m => m.Id == manufacturerId)
               .FirstOrDefaultAsync();

            foreach (Supplement supplement in manufacturer.Supplements)
            {
                supplement.IsDeleted = true;
            }

            manufacturer.IsDeleted = true;

            await this.database.SaveChangesAsync();
        }

        public async Task RestoreAsync(int manufacturerId)
        {
            Manufacturer manufacturer = await this.database
               .Manufacturers
               .Include(m => m.Supplements)
               .ThenInclude(s => s.Subcategory)
               .Where(m => m.Id == manufacturerId)
               .FirstOrDefaultAsync();

            foreach (Supplement supplement in manufacturer.Supplements)
            {
                if (!supplement.Subcategory.IsDeleted)
                {
                    supplement.IsDeleted = false;
                }
            }

            manufacturer.IsDeleted = false;

            await this.database.SaveChangesAsync();
        }
    }
}