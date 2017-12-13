namespace FitStore.Services.Contracts
{
    using Models.Comments;
    using System.Threading.Tasks;

    public interface ICommentService
    {
        Task CreateAsync(string content, string authorId, int supplementId);

        Task<CommentBasicServiceModel> GetEditModelAsync(int commentId);

        Task EditAsync(int commentId, string content);

        Task DeleteAsync(int commentId);

        Task<bool> IsUserAuthor(int commentId, string authorId);

        Task<bool> IsCommentModified(int commentId, string content);

        Task<bool> IsCommentExistingById(int commentId, bool isDeleted);
    }
}