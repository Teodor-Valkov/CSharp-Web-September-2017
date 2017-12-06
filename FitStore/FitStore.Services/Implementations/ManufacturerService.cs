namespace FitStore.Services.Implementations
{
    using AutoMapper.QueryableExtensions;
    using Contracts;
    using Data;
    using Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Models.Manufacturers;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class ManufacturerService : IManufacturerService
    {
        private readonly FitStoreDbContext database;

        public ManufacturerService(FitStoreDbContext database)
        {
            this.database = database;
        }

        public async Task<IEnumerable<ManufacturerBasicServiceModel>> GetAllBasicListingAsync()
        {
            return await this.database
               .Manufacturers
               .ProjectTo<ManufacturerBasicServiceModel>()
               .ToListAsync();
        }

        public async Task<ManufacturerDetailsServiceModel> GetDetailsByIdAsync(int manufacturerId)
        {
            return await this.database
              .Manufacturers
              .Where(m => m.Id == manufacturerId)
              .ProjectTo<ManufacturerDetailsServiceModel>()
              .FirstOrDefaultAsync();
        }

        public async Task<bool> IsManufacturerExistingById(int manufacturerId)
        {
            return await this.database
                .Manufacturers
                .AnyAsync(m => m.Id == manufacturerId);
        }

        public async Task<bool> IsManufacturerExistingByName(string name)
        {
            return await this.database
                .Manufacturers
                .AnyAsync(m => m.Name.ToLower() == name.ToLower());
        }
    }
}