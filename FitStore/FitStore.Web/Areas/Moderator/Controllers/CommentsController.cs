namespace FitStore.Web.Areas.Moderator.Controllers
{
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Services.Contracts;
    using Services.Moderator.Contracts;
    using System.Threading.Tasks;

    using static Common.CommonConstants;
    using static Common.CommonMessages;
    using static WebConstants;

    public class CommentsController : BaseModeratorController
    {
        private readonly IModeratorCommentService moderatorCommentService;
        private readonly ICommentService commentService;
        private readonly ISupplementService supplementService;

        public CommentsController(IModeratorCommentService moderatorCommentService, ICommentService commentService, ISupplementService supplementService)
        {
            this.moderatorCommentService = moderatorCommentService;
            this.commentService = commentService;
            this.supplementService = supplementService;
        }

        public async Task<IActionResult> Restore(int id, int supplementId)
        {
            bool isCommentExistingById = await this.commentService.IsCommentExistingById(id, true);

            if (!isCommentExistingById)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, CommentEntity));

                return this.RedirectToHomeIndex();
            }

            bool isSupplementExistingById = await this.supplementService.IsSupplementExistingById(supplementId, true);

            if (!isSupplementExistingById)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, SupplementEntity));

                return this.RedirectToHomeIndex();
            }

            bool isUserModerator = User.IsInRole(ModeratorRole);

            if (!isUserModerator)
            {
                return this.RedirectToHomeIndex();
            }

            bool restoreResult = await this.moderatorCommentService.RestoreAsync(id);

            if (restoreResult)
            {
                TempData.AddSuccessMessage(string.Format(EntityRestored, CommentEntity));
            }
            else
            {
                TempData.AddErrorMessage(string.Format(EntityNotRestored, CommentEntity));
            }

            return RedirectToAction(nameof(SupplementsController.Details), Supplements, new { area = ModeratorArea, id = supplementId });
        }
    }
}