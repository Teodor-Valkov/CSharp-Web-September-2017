namespace FitStore.Web.Areas.Moderator.Controllers
{
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Services.Contracts;
    using Services.Moderator.Contracts;
    using System.Threading.Tasks;
    using Web.Controllers;

    using static Common.CommonConstants;
    using static Common.CommonMessages;
    using static WebConstants;

    public class CommentsController : BaseModeratorController
    {
        private readonly IModeratorCommentService moderatorCommentService;
        private readonly ICommentService commentService;

        public CommentsController(IModeratorCommentService moderatorCommentService, ICommentService commentService)
        {
            this.moderatorCommentService = moderatorCommentService;
            this.commentService = commentService;
        }

        public async Task<IActionResult> Restore(int id, int supplementId)
        {
            bool isCommentExistingById = await this.commentService.IsCommentExistingById(id, true);

            if (!isCommentExistingById)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, CommentEntity));

                return this.RedirectToAction(nameof(SupplementsController.Details), Supplements, new { area = ModeratorArea, id = supplementId });
            }

            bool isUserModerator = User.IsInRole(ModeratorRole);

            if (!isUserModerator)
            {
                return RedirectToAction(nameof(HomeController.Index), Home);
            }

            await this.moderatorCommentService.RestoreAsync(id);

            TempData.AddSuccessMessage(string.Format(EntityRestored, CommentEntity));

            return RedirectToAction(nameof(SupplementsController.Details), Supplements, new { area = ModeratorArea, id = supplementId });
        }
    }
}