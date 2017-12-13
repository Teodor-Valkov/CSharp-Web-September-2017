namespace FitStore.Web.Areas.Moderator.Controllers
{
    using FitStore.Data.Models;
    using FitStore.Services.Contracts;
    using FitStore.Services.Moderator.Contracts;
    using FitStore.Services.Moderator.Models.Users;
    using FitStore.Web.Infrastructure.Extensions;
    using FitStore.Web.Models.Pagination;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    using static Common.CommonConstants;
    using static Common.CommonMessages;

    public class UsersController : BaseModeratorController
    {
        private readonly UserManager<User> userManager;
        private readonly IModeratorUserService moderatorUserService;
        private readonly IUserService userService;

        public UsersController(UserManager<User> userManager, IModeratorUserService moderatorUserService, IUserService userService)
        {
            this.userManager = userManager;
            this.moderatorUserService = moderatorUserService;
            this.userService = userService;
        }

        public async Task<IActionResult> Index(string searchToken, int page = 1)
        {
            if (page < 1)
            {
                return RedirectToAction(nameof(Index));
            }

            PagingElementsViewModel<ModeratorUserBasicServiceModel> model = new PagingElementsViewModel<ModeratorUserBasicServiceModel>
            {
                Elements = await this.moderatorUserService.GetAllListingAsync(searchToken, page),
                SearchToken = searchToken,
                Pagination = new PaginationViewModel
                {
                    TotalElements = await this.moderatorUserService.TotalCountAsync(searchToken),
                    PageSize = UserPageSize,
                    CurrentPage = page
                }
            };

            if (page > model.Pagination.TotalPages && model.Pagination.TotalPages != 0)
            {
                return RedirectToAction(nameof(Index), new { page = model.Pagination.TotalPages });
            }

            return View(model);
        }

        public async Task<IActionResult> Permission(string username)
        {
            bool isUserExisting = await this.userManager.FindByNameAsync(username) != null;

            if (!isUserExisting)
            {
                TempData.AddErrorMessage(InvalidIdentityDetailsErroMessage);

                return RedirectToAction(nameof(Index));
            }

            User user = await this.userManager.FindByNameAsync(username);

            await this.moderatorUserService.ChangePermission(user);

            return RedirectToAction(nameof(Index));
        }
    }
}