namespace FitStore.Services.Moderator.Implementations
{
    using AutoMapper.QueryableExtensions;
    using Contracts;
    using Data;
    using Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Models.Users;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using static Common.CommonConstants;

    public class ModeratorUserService : IModeratorUserService
    {
        private readonly FitStoreDbContext database;

        public ModeratorUserService(FitStoreDbContext database)
        {
            this.database = database;
        }

        public async Task<IEnumerable<ModeratorUserBasicServiceModel>> GetAllListingAsync(string searchToken, int page)
        {
            IQueryable<User> users = this.database.Users;

            if (!string.IsNullOrWhiteSpace(searchToken))
            {
                users = users.Where(u => u.UserName.ToLower().Contains(searchToken.ToLower()));
            }

            return await users
               .OrderBy(u => u.UserName)
               .Skip((page - 1) * UserPageSize)
               .Take(UserPageSize)
               .ProjectTo<ModeratorUserBasicServiceModel>()
               .ToListAsync();
        }

        public async Task ChangePermission(User user)
        {
            user.IsRestricted = !user.IsRestricted;

            this.database.Users.Update(user);
            await this.database.SaveChangesAsync();
        }

        public async Task<int> TotalCountAsync(string searchToken)
        {
            if (string.IsNullOrWhiteSpace(searchToken))
            {
                return await this.database
                    .Users
                    .CountAsync();
            }

            return await this.database
              .Users
              .Where(u => u.UserName.ToLower().Contains(searchToken.ToLower()))
              .CountAsync();
        }
    }
}