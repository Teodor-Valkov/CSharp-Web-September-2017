namespace FitStore.Services.Contracts
{
    using Models.Reviews;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IReviewService
    {
        Task<IEnumerable<ReviewAdvancedServiceModel>> GetAllListingAsync(int page);

        Task CreateAsync(string content, int rating, string authorId, int supplementId);

        Task<ReviewDetailsServiceModel> GetDetailsByIdAsync(int reviewId);

        Task<ReviewBasicServiceModel> GetEditModelAsync(int reviewId);

        Task EditAsync(int reviewId, string content, int rating);

        Task DeleteAsync(int reviewId);

        Task<bool> IsUserAuthor(int reviewId, string authorId);

        Task<bool> IsReviewModified(int reviewId, string content, int rating);

        Task<bool> IsReviewExistingById(int reviewId, bool isDeleted);

        Task<int> TotalCountAsync(bool shouldSeeDeletedReviews);
    }
}