namespace CameraBazaar.Web.Controllers
{
    using Data.Models;
    using Infrastructure;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Models.Identity;
    using Services.Contracts;
    using Services.Models.Identity;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    [Authorize(Roles = GlobalConstants.AdminName)]
    public class IdentityController : Controller
    {
        private const int PageSize = 2;

        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IIdentityService identityService;

        public IdentityController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IIdentityService identityService)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.identityService = identityService;
        }

        public IActionResult All(int page = 1)
        {
            if (page < 1)
            {
                return RedirectToAction(nameof(All));
            }

            UserPageListViewModel model = new UserPageListViewModel
            {
                Users = this.identityService.GetAllListing(page, PageSize),
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(this.userManager.Users.Count() / (double)PageSize)
            };

            if (page > model.TotalPages && model.TotalPages != 0)
            {
                return RedirectToAction(nameof(All), new { page = model.TotalPages });
            }

            return View(model);
        }

        public async Task<IActionResult> Roles(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            User user = await this.userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            IdentityWithRolesBasicServiceModel model = this.identityService.GetUserWithRolesById(id);

            if (model == null)
            {
                return NotFound();
            }

            model.Roles = await this.userManager.GetRolesAsync(user);

            return View(model);
        }

        public IActionResult Create()
        {
            UserFormViewModel model = new UserFormViewModel();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool createResult = await this.identityService.Create(model.Username, model.Email, model.Password, model.Phone);

            if (createResult)
            {
                TempData["SuccessMessage"] = $"User {model.Username} has been created.";
            }

            return RedirectToAction(nameof(All));
        }

        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            IdentityEditServiceModel model = this.identityService.GetUserEditDetailsById(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, IdentityEditServiceModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            ICollection<IdentityError> errors = await this.identityService.Edit(id, model.Email, model.Password, model.Phone);

            if (errors.Any())
            {
                foreach (IdentityError error in errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }

            TempData["SuccessMessage"] = $"User {model.Username} has been edited.";
            return RedirectToAction(nameof(All));
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            bool deleteResult = await this.identityService.Delete(id);

            if (deleteResult)
            {
                TempData["SuccessMessage"] = $"User has been deleted.";
            }

            return RedirectToAction(nameof(All));
        }

        public IActionResult AddToRole(string id)
        {
            IEnumerable<SelectListItem> roles = this.roleManager
                .Roles
                .Select(r => new SelectListItem
                {
                    Value = r.Name,
                    Text = r.Name
                })
                .ToList();

            return View(roles);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToRole(string id, string role)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            bool isRoleExisting = await this.roleManager.RoleExistsAsync(role);

            if (!isRoleExisting)
            {
                return NotFound();
            }

            User user = await this.userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            IdentityResult addToRoleResult = await this.userManager.AddToRoleAsync(user, role);

            if (addToRoleResult.Succeeded)
            {
                TempData["SuccessMessage"] = $"User {user.UserName} has been added to role {role}.";
            }

            return RedirectToAction(nameof(All));
        }

        public async Task<IActionResult> RemoveFromRole(string id, string role)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(role))
            {
                return NotFound();
            }

            bool isRoleExisting = await this.roleManager.RoleExistsAsync(role);

            if (!isRoleExisting)
            {
                return NotFound();
            }

            User user = await this.userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            IdentityResult removeFromRoleResult = await this.userManager.RemoveFromRoleAsync(user, role);

            if (removeFromRoleResult.Succeeded)
            {
                TempData["SuccessMessage"] = $"User {user.UserName} has been removed from role {role}.";
            }

            return RedirectToAction(nameof(All));
        }

        public IActionResult ChangeUserPermission(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            this.identityService.ChangeUserPermission(id);

            return RedirectToAction(nameof(All));
        }
    }
}