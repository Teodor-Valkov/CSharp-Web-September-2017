namespace FitStore.Web.Areas.Moderator.Controllers
{
    using Data.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Services.Moderator.Contracts;
    using Services.Moderator.Models.Users;
    using System.Threading.Tasks;
    using Web.Infrastructure.Extensions;
    using Web.Models.Pagination;

    using static Common.CommonConstants;
    using static Common.CommonMessages;

    public class UsersController : BaseModeratorController
    {
        private readonly UserManager<User> userManager;
        private readonly IModeratorUserService moderatorUserService;

        public UsersController(UserManager<User> userManager, IModeratorUserService moderatorUserService)
        {
            this.userManager = userManager;
            this.moderatorUserService = moderatorUserService;
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

            if (page > 1 && page > model.Pagination.TotalPages)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        public async Task<IActionResult> Permission(string username)
        {
            User user = await this.userManager.FindByNameAsync(username);

            if (user == null)
            {
                TempData.AddErrorMessage(InvalidIdentityDetailsErroMessage);

                return RedirectToAction(nameof(Index));
            }

            await this.moderatorUserService.ChangePermission(user);

            return RedirectToAction(nameof(Index));
        }
    }
}