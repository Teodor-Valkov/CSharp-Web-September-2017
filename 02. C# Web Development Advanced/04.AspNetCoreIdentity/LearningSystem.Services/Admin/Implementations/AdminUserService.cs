namespace LearningSystem.Services.Admin.Contracts
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Data;
    using Data.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Models.Users;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using static Common.CommonConstants;

    public class AdminUserService : IAdminUserService
    {
        private readonly LearningSystemDbContext database;
        private readonly UserManager<User> userManager;

        public AdminUserService(LearningSystemDbContext database, UserManager<User> userManager)
        {
            this.database = database;
            this.userManager = userManager;
        }

        public async Task<IEnumerable<AdminUserBasicServiceModel>> GetAllListingAsync(string searchToken, int page)
        {
            IQueryable<User> users = this.database.Users;

            if (!string.IsNullOrWhiteSpace(searchToken))
            {
                users = users.Where(u =>
                    u.UserName.ToLower().Contains(searchToken.ToLower()) ||
                    u.Name.ToLower().Contains(searchToken.ToLower()));
            }

            return await users
               .OrderBy(u => u.UserName)
               .Skip((page - 1) * UserPageSize)
               .Take(UserPageSize)
               .ProjectTo<AdminUserBasicServiceModel>()
               .ToListAsync();
        }

        public async Task<IEnumerable<AdminUserIdAndNameServiceModel>> GetAllUserIdsAndNamesAsync()
        {
            IList<User> users = await this.userManager.GetUsersInRoleAsync(TrainerRole);

            return Mapper.Map<IList<AdminUserIdAndNameServiceModel>>(users);
        }

        public async Task<int> TotalCountAsync(string searchToken)
        {
            if (string.IsNullOrWhiteSpace(searchToken))
            {
                return await this.database.Users.CountAsync();
            }

            return await this.database
              .Users
              .Where(u => u.UserName.ToLower().Contains(searchToken.ToLower()) || u.Name.ToLower().Contains(searchToken.ToLower()))
              .CountAsync();
        }
    }
}