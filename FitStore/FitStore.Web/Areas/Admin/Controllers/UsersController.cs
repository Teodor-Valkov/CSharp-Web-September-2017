namespace FitStore.Web.Areas.Admin.Controllers
{
    using Data.Models;
    using FitStore.Services.Models.Orders;
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Models.Users;
    using Services.Admin.Contracts;
    using Services.Admin.Models.Users;
    using Services.Contracts;
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
        private readonly IUserService userService;
        private readonly IOrderService orderService;

        public UsersController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager, IAdminUserService adminUserService, IUserService userService, IOrderService orderService)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.adminUserService = adminUserService;
            this.userService = userService;
            this.orderService = orderService;
        }

        public async Task<IActionResult> Index(string searchToken, int page = MinPage)
        {
            if (page < MinPage)
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

            if (page > MinPage && page > model.Pagination.TotalPages)
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

            IEnumerable<SelectListItem> currentRoles = this.roleManager
                .Roles
                .Where(r => currentRolesAsString.Contains(r.Name))
                .Select(r => new SelectListItem
                {
                    Value = r.Name,
                    Text = r.Name
                })
                .ToList();

            IEnumerable<SelectListItem> allRoles = this.roleManager
                .Roles
                .Where(r => !currentRolesAsString.Contains(r.Name))
                .Select(r => new SelectListItem
                {
                    Value = r.Name,
                    Text = r.Name
                })
                .ToList();

            model.CurrentRoles = currentRoles;
            model.AllRoles = allRoles;

            return View(model);
        }

        public async Task<IActionResult> Orders(string username, int page = MinPage)
        {
            User user = await this.userManager.FindByNameAsync(username);

            if (user == null)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, UserEntity));

                return this.RedirectToAction(nameof(Index));
            }

            if (page < MinPage)
            {
                return RedirectToAction(nameof(Orders), new { username });
            }

            PagingElementViewModel<AdminUserOrdersServiceModel> model = new PagingElementViewModel<AdminUserOrdersServiceModel>
            {
                Element = await this.adminUserService.GetOrdersByUsernameAsync(username, page),
                Pagination = new PaginationViewModel
                {
                    TotalElements = await this.userService.TotalOrdersAsync(username),
                    PageSize = OrderPageSize,
                    CurrentPage = page
                }
            };

            if (page > MinPage && page > model.Pagination.TotalPages)
            {
                return RedirectToAction(nameof(Orders), new { username });
            }

            return View(model);
        }

        public async Task<IActionResult> Review(int id)
        {
            bool isOrderExistingById = await this.orderService.IsOrderExistingById(id);

            if (!isOrderExistingById)
            {
                return RedirectToAction(nameof(Index));
            }

            OrderDetailsServiceModel model = await this.orderService.GetDetailsByIdAsync(id);

            string username = await this.adminUserService.GetUsernameByOrderIdAsync(id);

            ViewData["Username"] = username;

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

            TempData.AddSuccessMessage(string.Format(AddToRoleSuccessMessage, model.Username, model.Role));

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

            TempData.AddSuccessMessage(string.Format(RemoveFromRoleSuccessMessage, model.Username, model.Role));

            return RedirectToAction(nameof(Details), new { model.Username });
        }
    }
}