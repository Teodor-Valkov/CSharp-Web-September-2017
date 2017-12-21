namespace FitStore.Services.Moderator.Implementations
{
    using Contracts;
    using Data;
    using Data.Models;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using System.Threading.Tasks;

    using static Common.CommonConstants;
    using static Common.CommonMessages;

    public class ModeratorCommentService : IModeratorCommentService
    {
        private readonly FitStoreDbContext database;

        public ModeratorCommentService(FitStoreDbContext database)
        {
            this.database = database;
        }

        public async Task<string> RestoreAsync(int commentId)
        {
            Comment comment = await this.database
                .Comments
                .Include(c => c.Supplement)
                .Where(c => c.Id == commentId)
                .FirstOrDefaultAsync();

            Supplement supplement = comment.Supplement;

            if (supplement.IsDeleted)
            {
                return string.Format(EntityNotExists, SupplementEntity);
            }

            comment.IsDeleted = false;

            await this.database.SaveChangesAsync();

            return string.Empty;
        }
    }
}