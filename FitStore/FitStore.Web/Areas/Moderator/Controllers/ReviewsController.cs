namespace FitStore.Web.Areas.Moderator.Controllers
{
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Models.Pagination;
    using Services.Contracts;
    using Services.Models.Reviews;
    using Services.Moderator.Contracts;
    using System.Threading.Tasks;

    using static Common.CommonConstants;
    using static Common.CommonMessages;
    using static WebConstants;

    public class ReviewsController : BaseModeratorController
    {
        private readonly IModeratorReviewService moderatorReviewService;
        private readonly IReviewService reviewService;

        public ReviewsController(IModeratorReviewService moderatorReviewService, IReviewService reviewService)
        {
            this.moderatorReviewService = moderatorReviewService;
            this.reviewService = reviewService;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            if (page < 1)
            {
                return RedirectToAction(nameof(Index));
            }

            PagingElementsViewModel<ReviewAdvancedServiceModel> model = new PagingElementsViewModel<ReviewAdvancedServiceModel>
            {
                Elements = await this.moderatorReviewService.GetAllListingAsync(page),
                Pagination = new PaginationViewModel
                {
                    TotalElements = await this.reviewService.TotalCountAsync(true),
                    PageSize = ReviewPageSize,
                    CurrentPage = page
                }
            };

            if (page > model.Pagination.TotalPages && model.Pagination.TotalPages != 0)
            {
                return RedirectToAction(nameof(Index), new { page = model.Pagination.TotalPages });
            }

            ViewData["ReturnUrl"] = this.RedirectToModeratorReviewIndex();

            return View(model);
        }

        public async Task<IActionResult> Restore(int id)
        {
            bool isReviewExistingById = await this.reviewService.IsReviewExistingById(id, true);

            if (!isReviewExistingById)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, ReviewEntity));
                return this.RedirectToHomeIndex();
            }

            bool isUserModerator = User.IsInRole(ModeratorRole);

            if (!isUserModerator)
            {
                return this.RedirectToHomeIndex();
            }

            string restoreResult = await this.moderatorReviewService.RestoreAsync(id);

            if (restoreResult == string.Empty)
            {
                TempData.AddSuccessMessage(string.Format(EntityRestored, ReviewEntity));
            }
            else
            {
                TempData.AddErrorMessage(string.Format(EntityNotRestored, ReviewEntity) + restoreResult);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}