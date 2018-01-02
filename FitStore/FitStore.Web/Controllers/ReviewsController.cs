namespace FitStore.Web.Controllers
{
    using Data.Models;
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Models.Pagination;
    using Models.Reviews;
    using Services.Contracts;
    using Services.Models.Reviews;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using static Common.CommonConstants;
    using static Common.CommonMessages;
    using static WebConstants;

    [Authorize]
    public class ReviewsController : BaseController
    {
        private readonly UserManager<User> userManager;
        private readonly IReviewService reviewService;
        private readonly ISupplementService supplementService;
        private readonly IUserService userService;

        public ReviewsController(UserManager<User> userManager, IReviewService reviewService, ISupplementService supplementService, IUserService userService)
        {
            this.userManager = userManager;
            this.reviewService = reviewService;
            this.supplementService = supplementService;
            this.userService = userService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(int page = MinPage)
        {
            if (page < MinPage)
            {
                return RedirectToAction(nameof(Index));
            }

            PagingElementsViewModel<ReviewAdvancedServiceModel> model = new PagingElementsViewModel<ReviewAdvancedServiceModel>
            {
                Elements = await this.reviewService.GetAllListingAsync(page),
                Pagination = new PaginationViewModel
                {
                    TotalElements = await this.reviewService.TotalCountAsync(false),
                    PageSize = ReviewPageSize,
                    CurrentPage = page
                }
            };

            if (page > MinPage && page > model.Pagination.TotalPages)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            bool isReviewExistingById = await this.reviewService.IsReviewExistingById(id, false);

            if (!isReviewExistingById)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, ReviewEntity));

                return this.RedirectToHomeIndex();
            }

            ReviewDetailsServiceModel model = await this.reviewService.GetDetailsByIdAsync(id);

            return View(model);
        }

        public async Task<IActionResult> Create(int id)
        {
            bool isSupplementExisting = await this.supplementService.IsSupplementExistingById(id, false);

            if (!isSupplementExisting)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, SupplementEntity));

                return this.RedirectToHomeIndex();
            }

            string username = User.Identity.Name;

            bool isUserRestricted = await this.userService.IsUserRestricted(username);

            if (isUserRestricted)
            {
                TempData.AddErrorMessage(UserRestrictedErrorMessage);

                return this.RedirectToHomeIndex();
            }

            ReviewFormViewModel model = new ReviewFormViewModel()
            {
                Ratings = this.GetRatingsSelectListItems()
            };

            ViewData["SupplementId"] = id;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(int id, ReviewFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Ratings = this.GetRatingsSelectListItems();

                ViewData["SupplementId"] = id;

                return View(model);
            }

            bool isSupplementExisting = await this.supplementService.IsSupplementExistingById(id, false);

            if (!isSupplementExisting)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, SupplementEntity));

                return this.RedirectToHomeIndex();
            }

            string username = User.Identity.Name;

            bool isUserRestricted = await this.userService.IsUserRestricted(username);

            if (isUserRestricted)
            {
                TempData.AddErrorMessage(UserRestrictedErrorMessage);

                return this.RedirectToHomeIndex();
            }

            string userId = this.userManager.GetUserId(User);

            await this.reviewService.CreateAsync(model.Content, model.Rating, userId, id);

            TempData.AddSuccessMessage(string.Format(EntityCreated, ReviewEntity));

            bool isUserModerator = User.IsInRole(ModeratorRole);

            if (isUserModerator)
            {
                return RedirectToAction(nameof(ReviewsController.Index), Reviews, new { area = ModeratorArea });
            }

            return this.RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            bool isReviewExistingById = await this.reviewService.IsReviewExistingById(id, false);

            if (!isReviewExistingById)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, ReviewEntity));

                return this.RedirectToHomeIndex();
            }

            string username = User.Identity.Name;

            bool isUserRestricted = await this.userService.IsUserRestricted(username);

            if (isUserRestricted)
            {
                TempData.AddErrorMessage(UserRestrictedErrorMessage);

                return this.RedirectToHomeIndex();
            }

            string userId = this.userManager.GetUserId(User);

            bool isUserAuthor = await this.reviewService.IsUserAuthor(id, userId);
            bool isUserModerator = User.IsInRole(ModeratorRole);

            if (!isUserAuthor && !isUserModerator)
            {
                return this.RedirectToHomeIndex();
            }

            ReviewBasicServiceModel review = await this.reviewService.GetEditModelAsync(id);

            ReviewFormViewModel model = new ReviewFormViewModel()
            {
                Content = review.Content,
                Rating = review.Rating,
                Ratings = this.GetRatingsSelectListItems()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ReviewFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Ratings = this.GetRatingsSelectListItems();

                return View(model);
            }

            bool isReviewExistingById = await this.reviewService.IsReviewExistingById(id, false);

            if (!isReviewExistingById)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, ReviewEntity));

                return this.RedirectToHomeIndex();
            }

            string username = User.Identity.Name;

            bool isUserRestricted = await this.userService.IsUserRestricted(username);

            if (isUserRestricted)
            {
                TempData.AddErrorMessage(UserRestrictedErrorMessage);

                return this.RedirectToHomeIndex();
            }

            string userId = this.userManager.GetUserId(User);

            bool isUserAuthor = await this.reviewService.IsUserAuthor(id, userId);
            bool isUserModerator = User.IsInRole(ModeratorRole);

            if (!isUserAuthor && !isUserModerator)
            {
                return this.RedirectToHomeIndex();
            }

            bool isReviewModified = await this.reviewService.IsReviewModified(id, model.Content, model.Rating);

            if (!isReviewModified)
            {
                model.Ratings = this.GetRatingsSelectListItems();

                TempData.AddWarningMessage(EntityNotModified);

                return View(model);
            }

            await this.reviewService.EditAsync(id, model.Content, model.Rating);

            TempData.AddSuccessMessage(string.Format(EntityModified, ReviewEntity));

            if (isUserModerator)
            {
                return RedirectToAction(nameof(ReviewsController.Index), Reviews, new { area = ModeratorArea });
            }

            return this.RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            bool isReviewExistingById = await this.reviewService.IsReviewExistingById(id, false);

            if (!isReviewExistingById)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, ReviewEntity));

                return this.RedirectToHomeIndex();
            }

            string username = User.Identity.Name;

            bool isUserRestricted = await this.userService.IsUserRestricted(username);

            if (isUserRestricted)
            {
                TempData.AddErrorMessage(UserRestrictedErrorMessage);

                return this.RedirectToHomeIndex();
            }

            string userId = this.userManager.GetUserId(User);

            bool isUserAuthor = await this.reviewService.IsUserAuthor(id, userId);
            bool isUserModerator = User.IsInRole(ModeratorRole);

            if (!isUserAuthor && !isUserModerator)
            {
                return this.RedirectToHomeIndex();
            }

            await this.reviewService.DeleteAsync(id);

            TempData.AddSuccessMessage(string.Format(EntityDeleted, ReviewEntity));

            if (isUserModerator)
            {
                return RedirectToAction(nameof(ReviewsController.Index), Reviews, new { area = ModeratorArea });
            }

            return RedirectToAction(nameof(Index));
        }

        private IEnumerable<SelectListItem> GetRatingsSelectListItems()
        {
            IEnumerable<int> ratings = Enumerable.Range(1, 10);

            return ratings.Select(c => new SelectListItem
            {
                Value = c.ToString(),
                Text = c.ToString()
            })
            .ToList();
        }
    }
}