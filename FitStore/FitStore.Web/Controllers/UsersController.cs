namespace FitStore.Web.Controllers
{
    using Data.Models;
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Models.Pagination;
    using Services.Contracts;
    using Services.Models.Users;
    using System.Threading.Tasks;

    using static Common.CommonConstants;
    using static Common.CommonMessages;

    [Authorize]
    public class UsersController : BaseController
    {
        private readonly UserManager<User> userManager;
        private readonly IUserService userService;

        public UsersController(UserManager<User> userManager, IUserService userService)
        {
            this.userManager = userManager;
            this.userService = userService;
        }

        public async Task<IActionResult> Profile(string username, int page = MinPage)
        {
            User user = await this.userManager.FindByNameAsync(username);

            if (user == null)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, username));

                return this.RedirectToHomeIndex();
            }

            if (page < MinPage)
            {
                return RedirectToAction(nameof(Profile), new { username });
            }

            PagingElementViewModel<UserProfileServiceModel> model = new PagingElementViewModel<UserProfileServiceModel>
            {
                Element = await this.userService.GetProfileByUsernameAsync(username, page),
                Pagination = new PaginationViewModel
                {
                    TotalElements = await this.userService.TotalOrdersAsync(username),
                    PageSize = OrderPageSize,
                    CurrentPage = page
                }
            };

            if (page > MinPage && page > model.Pagination.TotalPages)
            {
                return RedirectToAction(nameof(Profile), new { username });
            }

            return View(model);
        }

        public async Task<IActionResult> EditProfile(string username)
        {
            User user = await this.userManager.FindByNameAsync(username);

            if (user == null)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, username));

                return this.RedirectToHomeIndex();
            }

            UserEditProfileServiceModel model = await this.userService.GetEditProfileByUsernameAsync(username);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(string username, UserEditProfileServiceModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            User user = await this.userManager.FindByNameAsync(username);

            if (user == null)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, username));

                return this.RedirectToHomeIndex();
            }

            bool editProfileResult = await this.userService.EditProfileAsync(user, model.FullName, model.Email, model.Address, model.PhoneNumber, model.BirthDate);

            if (!editProfileResult)
            {
                TempData.AddWarningMessage(EntityNotModified);

                return View(model);
            }

            TempData.AddSuccessMessage(UserEditProfileSuccessMessage);

            return RedirectToAction(nameof(Profile), new { username });
        }

        public async Task<IActionResult> ChangePassword(string username)
        {
            User user = await this.userManager.FindByNameAsync(username);

            if (user == null)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, username));

                return this.RedirectToHomeIndex();
            }

            bool userHasPassword = await this.userManager.HasPasswordAsync(user);

            if (!userHasPassword)
            {
                TempData.AddErrorMessage(UserChangePasswordExternalLoginErrorMessage);

                return RedirectToAction(nameof(Profile), new { username });
            }

            UserChangePasswordServiceModel model = await this.userService.GetChangePasswordByUsernameAsync(username);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string username, UserChangePasswordServiceModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            User user = await this.userManager.FindByNameAsync(username);

            if (user == null)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, username));

                return this.RedirectToHomeIndex();
            }

            bool userHasPassword = await this.userManager.HasPasswordAsync(user);

            if (!userHasPassword)
            {
                TempData.AddErrorMessage(UserChangePasswordExternalLoginErrorMessage);

                return RedirectToAction(nameof(Profile), new { username });
            }

            bool oldPasswordIsValid = await this.userService.IsOldPasswordValid(user, model.OldPassword);

            if (!oldPasswordIsValid)
            {
                TempData.AddErrorMessage(UserOldPasswordNotValid);

                return View(model);
            }

            bool changePasswordResult = await this.userService.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (!changePasswordResult)
            {
                TempData.AddWarningMessage(EntityNotModified);

                return View(model);
            }

            TempData.AddSuccessMessage(UserChangePasswordSuccessMessage);

            return RedirectToAction(nameof(Profile), new { username });
        }
    }
}