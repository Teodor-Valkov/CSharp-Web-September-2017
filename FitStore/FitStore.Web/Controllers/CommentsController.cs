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
    public class CommentsController : BaseController
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

        public async Task<IActionResult> Create(int id, string returnUrl)
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

            CommentFormViewModel model = new CommentFormViewModel()
            {
                SupplementId = id
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string returnUrl, CommentFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool isSupplementExisting = await this.supplementService.IsSupplementExistingById(model.SupplementId, false);

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

            await this.commentService.CreateAsync(model.Content, userId, model.SupplementId);

            TempData.AddSuccessMessage(string.Format(EntityCreated, CommentEntity));

            bool isUserModerator = User.IsInRole(ModeratorRole);

            if (isUserModerator)
            {
                return RedirectToAction(nameof(SupplementsController.Details), Supplements, new { area = ModeratorArea, id = model.SupplementId, returnUrl });
            }

            return this.RedirectToAction(nameof(SupplementsController.Details), Supplements, new { id = model.SupplementId, returnUrl });
        }

        public async Task<IActionResult> Edit(int id, int supplementId, string returnUrl)
        {
            bool isCommentExistingById = await this.commentService.IsCommentExistingById(id, false);

            if (!isCommentExistingById)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, CommentEntity));

                return this.RedirectToHomeIndex();
            }

            bool isSupplementExistingById = await this.supplementService.IsSupplementExistingById(supplementId, false);

            if (!isSupplementExistingById)
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

            bool isUserAuthor = await this.commentService.IsUserAuthor(id, userId);
            bool isUserModerator = User.IsInRole(ModeratorRole);

            if (!isUserAuthor && !isUserModerator)
            {
                return this.RedirectToHomeIndex();
            }

            CommentBasicServiceModel comment = await this.commentService.GetEditModelAsync(id);

            CommentFormViewModel model = Mapper.Map<CommentFormViewModel>(comment);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, string returnUrl, CommentFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool isCommentExistingById = await this.commentService.IsCommentExistingById(id, false);

            if (!isCommentExistingById)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, CommentEntity));

                return this.RedirectToHomeIndex();
            }

            bool isSupplementExistingById = await this.supplementService.IsSupplementExistingById(model.SupplementId, false);

            if (!isSupplementExistingById)
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

            bool isUserAuthor = await this.commentService.IsUserAuthor(id, userId);
            bool isUserModerator = User.IsInRole(ModeratorRole);

            if (!isUserAuthor && !isUserModerator)
            {
                return this.RedirectToHomeIndex();
            }

            bool isCommentModified = await this.commentService.IsCommentModified(id, model.Content);

            if (!isCommentModified)
            {
                TempData.AddWarningMessage(EntityNotModified);

                return View(model);
            }

            await this.commentService.EditAsync(id, model.Content);

            TempData.AddSuccessMessage(string.Format(EntityModified, CommentEntity));

            if (isUserModerator)
            {
                return RedirectToAction(nameof(SupplementsController.Details), Supplements, new { area = ModeratorArea, id = model.SupplementId, returnUrl });
            }

            return this.RedirectToAction(nameof(SupplementsController.Details), Supplements, new { id = model.SupplementId, returnUrl });
        }

        public async Task<IActionResult> Delete(int id, int supplementId, string returnUrl)
        {
            bool isCommentExistingById = await this.commentService.IsCommentExistingById(id, false);

            if (!isCommentExistingById)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, CommentEntity));

                return this.RedirectToHomeIndex();
            }

            bool isSupplementExistingById = await this.supplementService.IsSupplementExistingById(supplementId, false);

            if (!isSupplementExistingById)
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

            bool isUserAuthor = await this.commentService.IsUserAuthor(id, userId);
            bool isUserModerator = User.IsInRole(ModeratorRole);

            if (!isUserAuthor && !isUserModerator)
            {
                return this.RedirectToHomeIndex();
            }

            await this.commentService.DeleteAsync(id);

            TempData.AddSuccessMessage(string.Format(EntityDeleted, CommentEntity));

            if (isUserModerator)
            {
                return RedirectToAction(nameof(SupplementsController.Details), Supplements, new { area = ModeratorArea, id = supplementId, returnUrl });
            }

            return this.RedirectToAction(nameof(SupplementsController.Details), Supplements, new { id = supplementId, returnUrl });
        }
    }
}