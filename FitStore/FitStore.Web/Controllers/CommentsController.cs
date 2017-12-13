namespace FitStore.Web.Controllers
{
    using AutoMapper;
    using Data.Models;
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Models.Comments;
    using Services.Contracts;
    using Services.Models.Comments;
    using System.Threading.Tasks;

    using static Common.CommonConstants;
    using static Common.CommonMessages;
    using static WebConstants;

    [Authorize]
    public class CommentsController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly ICommentService commentService;
        private readonly ISupplementService supplementService;
        private readonly IUserService userService;

        public CommentsController(UserManager<User> userManager, ICommentService commentService, ISupplementService supplementService, IUserService userService)
        {
            this.userManager = userManager;
            this.commentService = commentService;
            this.supplementService = supplementService;
            this.userService = userService;
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

            CommentFormViewModel model = new CommentFormViewModel
            {
                SupplementId = id
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(int id, CommentFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
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

            await this.commentService.CreateAsync(model.Content, userId, id);

            TempData.AddSuccessMessage(string.Format(EntityCreated, CommentEntity));

            return this.RedirectToAction(nameof(SupplementsController.Details), Supplements, new { id });
        }

        public async Task<IActionResult> Edit(int id, int supplementId)
        {
            bool isCommentExistingById = await this.commentService.IsCommentExistingById(id, false);

            if (!isCommentExistingById)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, CommentEntity));

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

            bool isUserAuthor = await this.commentService.IsUserAuthor(id, userId);
            bool isUserModerator = User.IsInRole(ModeratorRole);

            if (!isUserAuthor && !isUserModerator)
            {
                return RedirectToAction(nameof(HomeController.Index), Home);
            }

            CommentBasicServiceModel comment = await this.commentService.GetEditModelAsync(id);

            CommentFormViewModel model = Mapper.Map<CommentFormViewModel>(comment);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, int supplementId, CommentFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool isCommentExistingById = await this.commentService.IsCommentExistingById(id, false);

            if (!isCommentExistingById)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, CommentEntity));

                return this.RedirectToAction(nameof(SupplementsController.Details), Supplements, new { id = supplementId });
            }

            string username = User.Identity.Name;

            bool isUserRestricted = await this.userService.IsUserRestricted(username);

            if (isUserRestricted)
            {
                TempData.AddErrorMessage(UserRestrictedErrorMessage);

                return RedirectToAction(nameof(HomeController.Index), Home);
            }

            string userId = this.userManager.GetUserId(User);

            bool isUserAuthor = await this.commentService.IsUserAuthor(id, userId);
            bool isUserModerator = User.IsInRole(ModeratorRole);

            if (!isUserAuthor && !isUserModerator)
            {
                return RedirectToAction(nameof(HomeController.Index), Home);
            }

            bool isCommentModified = await this.commentService.IsCommentModified(id, model.Content);

            if (!isCommentModified)
            {
                TempData.AddWarningMessage(EntityNotModified);

                return View(model);
            }

            await this.commentService.EditAsync(id, model.Content);

            TempData.AddSuccessMessage(string.Format(EntityModified, CommentEntity));

            return this.RedirectToAction(nameof(SupplementsController.Details), Supplements, new { id = supplementId });
        }

        public async Task<IActionResult> Delete(int id, int supplementId)
        {
            bool isCommentExistingById = await this.commentService.IsCommentExistingById(id, false);

            if (!isCommentExistingById)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, CommentEntity));

                return this.RedirectToAction(nameof(SupplementsController.Details), Supplements, new { area = ModeratorArea, id = supplementId });
            }

            string username = User.Identity.Name;

            bool isUserRestricted = await this.userService.IsUserRestricted(username);

            if (isUserRestricted)
            {
                TempData.AddErrorMessage(UserRestrictedErrorMessage);

                return RedirectToAction(nameof(HomeController.Index), Home);
            }

            string userId = this.userManager.GetUserId(User);

            bool isUserAuthor = await this.commentService.IsUserAuthor(id, userId);
            bool isUserModerator = User.IsInRole(ModeratorRole);

            if (!isUserAuthor && !isUserModerator)
            {
                return RedirectToAction(nameof(HomeController.Index), Home);
            }

            await this.commentService.DeleteAsync(id);

            TempData.AddSuccessMessage(string.Format(EntityDeleted, CommentEntity));

            return this.RedirectToAction(nameof(SupplementsController.Details), Supplements, new { id = supplementId });
        }
    }
}