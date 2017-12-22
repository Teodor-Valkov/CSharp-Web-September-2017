namespace FitStore.Services.Manager.Implementations
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

    using static Common.CommonConstants;

    public class ManagerManufacturerService : IManagerManufacturerService
    {
        private readonly FitStoreDbContext database;

        public ManagerManufacturerService(FitStoreDbContext database)
        {
            this.database = database;
        }

        public async Task<IEnumerable<ManufacturerAdvancedServiceModel>> GetAllPagedListingAsync(bool isDeleted, string searchToken, int page)
        {
            IQueryable<Manufacturer> manufacturers = this.database.Manufacturers;

            if (!string.IsNullOrWhiteSpace(searchToken))
            {
                manufacturers = manufacturers.Where(c => c.Name.ToLower().Contains(searchToken.ToLower()));
            }

            return await manufacturers
               .Where(m => m.IsDeleted == isDeleted)
               .OrderBy(m => m.Name)
               .Skip((page - 1) * ManufacturerPageSize)
               .Take(ManufacturerPageSize)
               .ProjectTo<ManufacturerAdvancedServiceModel>()
               .ToListAsync();
        }

        public async Task<IEnumerable<ManufacturerBasicServiceModel>> GetAllBasicListingAsync(bool isDeleted)
        {
            return await this.database
               .Manufacturers
               .Where(m => m.IsDeleted == isDeleted)
               .OrderBy(m => m.Name)
               .ProjectTo<ManufacturerBasicServiceModel>()
               .ToListAsync();
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
                   .ThenInclude(s => s.Reviews)
               .Include(m => m.Supplements)
                   .ThenInclude(s => s.Comments)
               .Where(m => m.Id == manufacturerId)
               .FirstOrDefaultAsync();

            foreach (Supplement supplement in manufacturer.Supplements)
            {
                foreach (Review review in supplement.Reviews)
                {
                    review.IsDeleted = true;
                }

                foreach (Comment comment in supplement.Comments)
                {
                    comment.IsDeleted = true;
                }

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
               .Include(m => m.Supplements)
                   .ThenInclude(s => s.Reviews)
               .Include(m => m.Supplements)
                   .ThenInclude(s => s.Comments)
               .Where(m => m.Id == manufacturerId)
               .FirstOrDefaultAsync();

            foreach (Supplement supplement in manufacturer.Supplements)
            {
                if (!supplement.Subcategory.IsDeleted)
                {
                    foreach (Review review in supplement.Reviews)
                    {
                        review.IsDeleted = false;
                    }

                    foreach (Comment comment in supplement.Comments)
                    {
                        comment.IsDeleted = false;
                    }

                    supplement.IsDeleted = false;
                }
            }

            manufacturer.IsDeleted = false;

            await this.database.SaveChangesAsync();
        }

        public async Task<bool> IsManufacturerModified(int manufacturerId, string name, string address)
        {
            Manufacturer manufacturer = await this.database
             .Manufacturers
             .Where(m => m.Id == manufacturerId)
             .FirstOrDefaultAsync();

            if (name == manufacturer.Name && address == manufacturer.Address)
            {
                return false;
            }

            return true;
        }

        public async Task<int> TotalCountAsync(bool isDeleted, string searchToken)
        {
            if (string.IsNullOrWhiteSpace(searchToken))
            {
                return await this.database
                    .Manufacturers
                    .Where(m => m.IsDeleted == isDeleted)
                    .CountAsync();
            }

            return await this.database
              .Manufacturers
              .Where(m => m.IsDeleted == isDeleted && m.Name.ToLower().Contains(searchToken.ToLower()))
              .CountAsync();
        }
    }
}