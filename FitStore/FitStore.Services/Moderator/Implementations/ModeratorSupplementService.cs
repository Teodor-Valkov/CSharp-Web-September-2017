namespace FitStore.Services.Moderator.Implementations
{
    using AutoMapper.QueryableExtensions;
    using Contracts;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Models.Supplements;
    using System.Linq;
    using System.Threading.Tasks;

    public class ModeratorSupplementService : IModeratorSupplementService
    {
        private readonly FitStoreDbContext database;

        public ModeratorSupplementService(FitStoreDbContext database)
        {
            this.database = database;
        }

        public async Task<SupplementDetailsWithDeletedCommentsServiceModel> GetDetailsWithDeletedCommentsByIdAsync(int supplementId, int page)
        {
            return await this.database
              .Supplements
              .Where(s => s.Id == supplementId)
              .ProjectTo<SupplementDetailsWithDeletedCommentsServiceModel>(new { page })
              .FirstOrDefaultAsync();
        }
    }
}