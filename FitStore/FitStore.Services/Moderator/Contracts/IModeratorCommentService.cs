namespace FitStore.Services.Moderator.Contracts
{
    using System.Threading.Tasks;

    public interface IModeratorCommentService
    {
        Task RestoreAsync(int commentId);
    }
}