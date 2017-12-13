namespace FitStore.Services.Moderator.Contracts
{
    using Models.Supplements;
    using System.Threading.Tasks;

    public interface IModeratorSupplementService
    {
        Task<SupplementDetailsWithDeletedCommentsServiceModel> GetDetailsWithDeletedCommentsByIdAsync(int supplementId, int page);
    }
}