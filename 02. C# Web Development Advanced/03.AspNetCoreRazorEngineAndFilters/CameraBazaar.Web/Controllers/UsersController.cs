namespace CameraBazaar.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Services.Contracts;
    using Services.Models.Users;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class UsersController : Controller
    {
        private IUserService userService;

        public UsersController(IUserService userService)
        {
            this.userService = userService;
        }

        [Authorize]
        public IActionResult Profile(string username)
        {
            if (username == null)
            {
                return NotFound();
            }

            UserDetailsServiceModel model = this.userService.GetUserDetailsByUsername(username);

            if (model == null)
            {
                return NotFound();
            }

            return this.View(model);
        }

        [Authorize]
        public IActionResult Edit(string username)
        {
            if (username.ToLower() != User.Identity.Name.ToLower())
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(username))
            {
                return NotFound();
            }

            UserEditServiceModel model = this.userService.GetUserEditDetailsByUsername(username);

            if (model == null)
            {
                return NotFound();
            }

            return this.View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string username, UserEditServiceModel model)
        {
            if (username.ToLower() != User.Identity.Name.ToLower())
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            ICollection<IdentityError> errors = await this.userService.Edit(username, model.Email, model.OldPassword, model.NewPassword, model.Phone);

            if (errors.Any())
            {
                foreach (IdentityError error in errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }

            return RedirectToAction(nameof(Profile), new { username = username });
        }
    }
}