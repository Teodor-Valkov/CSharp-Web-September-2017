namespace FitStore.Services.Implementations
{
    using AutoMapper.QueryableExtensions;
    using Contracts;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Models.Manufacturers;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using static Common.CommonConstants;

    public class ManufacturerService : IManufacturerService
    {
        private readonly FitStoreDbContext database;

        public ManufacturerService(FitStoreDbContext database)
        {
            this.database = database;
        }

        public async Task<IEnumerable<ManufacturerAdvancedServiceModel>> GetAllPagedListingAsync(int page)
        {
            return await this.database
                .Manufacturers
                .Where(m => m.IsDeleted == false)
                .OrderBy(m => m.Name)
                .Skip((page - 1) * ManufacturerPageSize)
                .Take(ManufacturerPageSize)
                .ProjectTo<ManufacturerAdvancedServiceModel>()
                .ToListAsync();
        }

        public async Task<ManufacturerDetailsServiceModel> GetDetailsByIdAsync(int manufacturerId, int page)
        {
            return await this.database
              .Manufacturers
              .Where(m => m.Id == manufacturerId)
              .ProjectTo<ManufacturerDetailsServiceModel>(new { page })
              .FirstOrDefaultAsync();
        }

        public async Task<bool> IsManufacturerExistingById(int manufacturerId, bool isDeleted)
        {
            return await this.database
                .Manufacturers
                .AnyAsync(m => m.Id == manufacturerId && m.IsDeleted == isDeleted);
        }

        public async Task<bool> IsManufacturerExistingByName(string name)
        {
            return await this.database
                .Manufacturers
                .AnyAsync(m => m.Name.ToLower() == name.ToLower());
        }

        public async Task<bool> IsManufacturerExistingByIdAndName(int manufacturerId, string name)
        {
            return await this.database
                .Manufacturers
                .AnyAsync(m => m.Id != manufacturerId && m.Name.ToLower() == name.ToLower());
        }

        public async Task<int> TotalSupplementsCountAsync(int manufacturerId)
        {
            return await this.database
                .Manufacturers
                .Where(m => m.Id == manufacturerId)
                .SumAsync(m => m.Supplements.Count(s => s.IsDeleted == false));
        }

        public async Task<int> TotalCountAsync()
        {
            return await this.database
                .Manufacturers
                .Where(m => m.IsDeleted == false)
                .CountAsync();
        }
    }
}