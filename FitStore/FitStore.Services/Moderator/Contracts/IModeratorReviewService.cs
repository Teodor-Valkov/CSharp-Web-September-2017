namespace FitStore.Services.Moderator.Contracts
{
    using Services.Models.Reviews;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IModeratorReviewService
    {
        Task<IEnumerable<ReviewAdvancedServiceModel>> GetAllListingAsync(int page);

        Task<string> RestoreAsync(int reviewId);
    }
}