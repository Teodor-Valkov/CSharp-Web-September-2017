namespace FitStore.Services.Manager.Implementations
{
    using AutoMapper.QueryableExtensions;
    using Contracts;
    using Data;
    using Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Models.Supplements;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using static Common.CommonConstants;

    public class ManagerSupplementService : IManagerSupplementService
    {
        private readonly FitStoreDbContext database;

        public ManagerSupplementService(FitStoreDbContext database)
        {
            this.database = database;
        }

        public async Task<IEnumerable<SupplementAdvancedServiceModel>> GetAllPagedListingAsync(bool isDeleted, string searchToken, int page)
        {
            IQueryable<Supplement> supplements = this.database.Supplements;

            if (!string.IsNullOrWhiteSpace(searchToken))
            {
                supplements = supplements.Where(s => s.Name.ToLower().Contains(searchToken.ToLower()));
            }

            return await supplements
               .Where(s => s.IsDeleted == isDeleted)
               .OrderBy(s => s.Name)
               .Skip((page - 1) * SupplementPageSize)
               .Take(SupplementPageSize)
               .ProjectTo<SupplementAdvancedServiceModel>()
               .ToListAsync();
        }

        public async Task CreateAsync(string name, string description, int quantity, decimal price, byte[] picture, DateTime bestBeforeDate, int subcategoryId, int manufacturerId)
        {
            Supplement supplement = new Supplement
            {
                Name = name,
                Description = description,
                Quantity = quantity,
                Price = price,
                Picture = picture,
                BestBeforeDate = bestBeforeDate,
                SubcategoryId = subcategoryId,
                ManufacturerId = manufacturerId
            };

            await this.database.Supplements.AddAsync(supplement);
            await this.database.SaveChangesAsync();
        }

        public async Task<SupplementServiceModel> GetEditModelAsync(int supplementId)
        {
            return await this.database
                .Supplements
                .Where(s => s.Id == supplementId)
                .ProjectTo<SupplementServiceModel>()
                .FirstOrDefaultAsync();
        }

        public async Task EditAsync(int supplementId, string name, string description, int quantity, decimal price, byte[] picture, DateTime bestBeforeDate, int subcategoryId, int manufacturerId)
        {
            Supplement supplement = await this.database
                .Supplements
                .Where(s => s.Id == supplementId)
                .FirstOrDefaultAsync();

            supplement.Name = name;
            supplement.Description = description;
            supplement.Quantity = quantity;
            supplement.Price = price;
            supplement.Picture = picture;
            supplement.BestBeforeDate = bestBeforeDate;
            supplement.SubcategoryId = subcategoryId;
            supplement.ManufacturerId = manufacturerId;

            this.database.Supplements.Update(supplement);
            await this.database.SaveChangesAsync();
        }

        public async Task DeleteAsync(int supplementId)
        {
            Supplement supplement = await this.database
                  .Supplements
                  .Where(s => s.Id == supplementId)
                  .FirstOrDefaultAsync();

            supplement.IsDeleted = true;

            await this.database.SaveChangesAsync();
        }

        public async Task RestoreAsync(int supplementId)
        {
            Supplement supplement = await this.database
                    .Supplements
                    .Where(s => s.Id == supplementId)
                    .FirstOrDefaultAsync();

            supplement.IsDeleted = false;

            await this.database.SaveChangesAsync();
        }

        public async Task<bool> IsSupplementModified(int supplementId, string name, string description, int quantity, decimal price, byte[] picture, DateTime bestBeforeDate, int subcategoryId, int manufacturerId)
        {
            Supplement supplement = await this.database
                  .Supplements
                  .Where(s => s.Id == supplementId)
                  .FirstOrDefaultAsync();

            if (name == supplement.Name && description == supplement.Description && quantity == supplement.Quantity
                && price == supplement.Price && picture.SequenceEqual(supplement.Picture) && bestBeforeDate == supplement.BestBeforeDate
                && subcategoryId == supplement.SubcategoryId && manufacturerId == supplement.ManufacturerId)
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
                    .Supplements
                    .Where(s => s.IsDeleted == isDeleted)
                    .CountAsync();
            }

            return await this.database
              .Supplements
              .Where(s => s.IsDeleted == isDeleted && s.Name.ToLower().Contains(searchToken.ToLower()))
              .CountAsync();
        }
    }
}