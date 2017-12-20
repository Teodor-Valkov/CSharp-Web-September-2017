namespace FitStore.Web.Areas.Admin.Controllers
{
    using Data.Models;
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using Models.Users;
    using Services.Admin.Contracts;
    using Services.Admin.Models.Users;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Web.Models.Pagination;

    using static Common.CommonConstants;
    using static Common.CommonMessages;
    using static WebConstants;

    [Area(AdministratorArea)]
    [Authorize(Roles = AdministratorRole)]
    public class UsersController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<User> userManager;
        private readonly IAdminUserService adminUserService;

        public UsersController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager, IAdminUserService adminUserService)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.adminUserService = adminUserService;
        }

        public async Task<IActionResult> Index(string searchToken, int page = 1)
        {
            if (page < 1)
            {
                return RedirectToAction(nameof(Index), new { searchToken });
            }

            PagingElementsViewModel<AdminUserBasicServiceModel> model = new PagingElementsViewModel<AdminUserBasicServiceModel>
            {
                Elements = await this.adminUserService.GetAllListingAsync(searchToken, page),
                SearchToken = searchToken,
                Pagination = new PaginationViewModel
                {
                    TotalElements = await this.adminUserService.TotalCountAsync(searchToken),
                    PageSize = UserPageSize,
                    CurrentPage = page
                }
            };

            if (page > 1 && page > model.Pagination.TotalPages)
            {
                return RedirectToAction(nameof(Index), new { searchToken });
            }

            return View(model);
        }

        public async Task<IActionResult> Details(string username)
        {
            User user = await this.userManager.FindByNameAsync(username);

            if (user == null)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, UserEntity));

                return this.RedirectToAction(nameof(Index));
            }

            AdminUserDetailsServiceModel model = await this.adminUserService.GetDetailsByUsernameAsync(username);

            IList<string> currentRolesAsString = await this.userManager.GetRolesAsync(user);

            IEnumerable<SelectListItem> currentRoles = await this.roleManager
                .Roles
                .Where(r => currentRolesAsString.Contains(r.Name))
                .Select(r => new SelectListItem
                {
                    Value = r.Name,
                    Text = r.Name
                })
                .ToListAsync();

            IEnumerable<SelectListItem> allRoles = await this.roleManager
                .Roles
                .Where(r => !currentRolesAsString.Contains(r.Name))
                .Select(r => new SelectListItem
                {
                    Value = r.Name,
                    Text = r.Name
                })
                .ToListAsync();

            model.CurrentRoles = currentRoles;
            model.AllRoles = allRoles;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddToRole(UserWithRoleFormViewModel model)
        {
            bool isRoleExisting = await this.roleManager.RoleExistsAsync(model.Role);

            User user = await this.userManager.FindByNameAsync(model.Username);

            if (!ModelState.IsValid || !isRoleExisting || user == null)
            {
                TempData.AddErrorMessage(InvalidIdentityDetailsErroMessage);

                return RedirectToAction(nameof(Details), new { model.Username });
            }

            IdentityResult addToRoleResult = await this.userManager.AddToRoleAsync(user, model.Role);

            if (!addToRoleResult.Succeeded)
            {
                string errors = string.Join(Environment.NewLine, addToRoleResult.Errors.Select(e => e.Description));

                TempData.AddErrorMessage(string.Format(ChangeRoleErrorMessage, errors));

                return RedirectToAction(nameof(Details), new { model.Username });
            }

            await this.userManager.UpdateAsync(user);

            TempData.AddSuccessMessage(string.Format(AddToRoleSuccessMessage, user.UserName, model.Role));

            return RedirectToAction(nameof(Details), new { model.Username });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromRole(UserWithRoleFormViewModel model)
        {
            bool isRoleExisting = await this.roleManager.RoleExistsAsync(model.Role);

            User user = await this.userManager.FindByNameAsync(model.Username);

            if (!ModelState.IsValid || !isRoleExisting || user == null)
            {
                TempData.AddErrorMessage(InvalidIdentityDetailsErroMessage);

                return RedirectToAction(nameof(Details), new { model.Username });
            }

            IdentityResult removeFromRoleResult = await this.userManager.RemoveFromRoleAsync(user, model.Role);

            if (!removeFromRoleResult.Succeeded)
            {
                string errors = string.Join(Environment.NewLine, removeFromRoleResult.Errors.Select(e => e.Description));

                TempData.AddErrorMessage(string.Format(ChangeRoleErrorMessage, errors));

                return RedirectToAction(nameof(Details), new { model.Username });
            }

            await this.userManager.UpdateAsync(user);

            TempData.AddSuccessMessage(string.Format(RemoveFromRoleSuccessMessage, user.UserName, model.Role));

            return RedirectToAction(nameof(Details), new { model.Username });
        }
    }
}