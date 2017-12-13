namespace FitStore.Web.Controllers
{
    using Data.Models;
    using FitStore.Services.Models.Reviews;
    using FitStore.Web.Models.Pagination;
    using FitStore.Web.Models.Reviews;
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Services.Contracts;
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
        public async Task<IActionResult> Index(int page = 1)
        {
            if (page < 1)
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

            if (page > model.Pagination.TotalPages && model.Pagination.TotalPages != 0)
            {
                return RedirectToAction(nameof(Index), new { page = model.Pagination.TotalPages });
            }

            ViewData["ReturnUrl"] = this.RedirectToHomeIndex();

            return View(model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            bool isReviewExistingById = await this.reviewService.IsReviewExistingById(id, false);

            if (!isReviewExistingById)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, ReviewEntity));

                return RedirectToAction(nameof(HomeController.Index), Home);
            }

            ReviewDetailsServiceModel model = await this.reviewService.GeDetailsByIdAsync(id);

            return View(model);
        }

        public async Task<IActionResult> Create(int id)
        {
            bool isSupplementExisting = await this.supplementService.IsSupplementExistingById(id, false);

            if (!isSupplementExisting)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, SupplementEntity));

                return RedirectToAction(nameof(HomeController.Index), Home);
            }

            string username = User.Identity.Name;

            bool isUserRestricted = await this.userService.IsUserRestricted(username);

            if (isUserRestricted)
            {
                TempData.AddErrorMessage(UserRestrictedErrorMessage);

                return RedirectToAction(nameof(HomeController.Index), Home);
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

                return View(model);
            }

            bool isSupplementExisting = await this.supplementService.IsSupplementExistingById(id, false);

            if (!isSupplementExisting)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, SupplementEntity));

                return RedirectToAction(nameof(HomeController.Index), Home);
            }

            string username = User.Identity.Name;

            bool isUserRestricted = await this.userService.IsUserRestricted(username);

            if (isUserRestricted)
            {
                TempData.AddErrorMessage(UserRestrictedErrorMessage);

                return RedirectToAction(nameof(HomeController.Index), Home);
            }

            string userId = this.userManager.GetUserId(User);

            await this.reviewService.CreateAsync(model.Content, model.Rating, userId, id);

            TempData.AddSuccessMessage(string.Format(EntityCreated, ReviewEntity));

            return this.RedirectToAction(nameof(ReviewsController.Index), Reviews);
        }

        public async Task<IActionResult> Edit(int id)
        {
            bool isReviewExistingById = await this.reviewService.IsReviewExistingById(id, false);

            if (!isReviewExistingById)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, ReviewEntity));

                return RedirectToAction(nameof(HomeController.Index), Home);
            }

            string username = User.Identity.Name;

            bool isUserRestricted = await this.userService.IsUserRestricted(username);

            if (isUserRestricted)
            {
                TempData.AddErrorMessage(UserRestrictedErrorMessage);

                return RedirectToAction(nameof(HomeController.Index), Home);
            }

            string userId = this.userManager.GetUserId(User);

            bool isUserAuthor = await this.reviewService.IsUserAuthor(id, userId);
            bool isUserModerator = User.IsInRole(ModeratorRole);

            if (!isUserAuthor && !isUserModerator)
            {
                return RedirectToAction(nameof(HomeController.Index), Home);
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

                return RedirectToAction(nameof(HomeController.Index), Home);
            }

            string username = User.Identity.Name;

            bool isUserRestricted = await this.userService.IsUserRestricted(username);

            if (isUserRestricted)
            {
                TempData.AddErrorMessage(UserRestrictedErrorMessage);

                return RedirectToAction(nameof(HomeController.Index), Home);
            }

            string userId = this.userManager.GetUserId(User);

            bool isUserAuthor = await this.reviewService.IsUserAuthor(id, userId);
            bool isUserModerator = User.IsInRole(ModeratorRole);

            if (!isUserAuthor && !isUserModerator)
            {
                return RedirectToAction(nameof(HomeController.Index), Home);
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

            return this.RedirectToAction(nameof(ReviewsController.Index), Reviews);
        }

        public async Task<IActionResult> Delete(int id)
        {
            bool isReviewExistingById = await this.reviewService.IsReviewExistingById(id, false);

            if (!isReviewExistingById)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, ReviewEntity));

                return RedirectToAction(nameof(HomeController.Index), Home);
            }

            string username = User.Identity.Name;

            bool isUserRestricted = await this.userService.IsUserRestricted(username);

            if (isUserRestricted)
            {
                TempData.AddErrorMessage(UserRestrictedErrorMessage);

                return RedirectToAction(nameof(HomeController.Index), Home);
            }

            string userId = this.userManager.GetUserId(User);

            bool isUserAuthor = await this.reviewService.IsUserAuthor(id, userId);
            bool isUserModerator = User.IsInRole(ModeratorRole);

            if (!isUserAuthor && !isUserModerator)
            {
                return RedirectToAction(nameof(HomeController.Index), Home);
            }

            await this.reviewService.DeleteAsync(id);

            TempData.AddSuccessMessage(string.Format(EntityDeleted, ReviewEntity));

            if (isUserModerator)
            {
                return RedirectToAction(nameof(ReviewsController.Index), Reviews, new { area = ModeratorArea });
            }

            return RedirectToAction(nameof(ReviewsController.Index), Reviews);
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