namespace FitStore.Services.Admin.Implementations
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

    public class AdminUserService : IAdminUserService
    {
        private readonly FitStoreDbContext database;

        public AdminUserService(FitStoreDbContext database)
        {
            this.database = database;
        }

        public async Task<IEnumerable<AdminUserBasicServiceModel>> GetAllListingAsync(string searchToken, int page)
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
               .ProjectTo<AdminUserBasicServiceModel>()
               .ToListAsync();
        }

        public async Task<AdminUserDetailsServiceModel> GetDetailsByUsernameAsync(string username)
        {
            return await this.database
              .Users
              .Where(u => u.UserName.ToLower() == username.ToLower())
              .ProjectTo<AdminUserDetailsServiceModel>()
              .FirstOrDefaultAsync();
        }

        public async Task<AdminUserOrdersServiceModel> GetOrdersByUsernameAsync(string username, int page)
        {
            return await this.database
              .Users
              .Where(u => u.UserName.ToLower() == username.ToLower())
              .ProjectTo<AdminUserOrdersServiceModel>(new { username, page })
              .FirstOrDefaultAsync();
        }

        public async Task<string> GetUsernameByOrderIdAsync(int orderId)
        {
            return await this.database
                .Orders
                .Where(o => o.Id == orderId)
                .Select(o => o.User.UserName)
                .FirstOrDefaultAsync();
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