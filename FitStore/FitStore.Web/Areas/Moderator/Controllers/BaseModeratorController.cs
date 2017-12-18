namespace FitStore.Web.Areas.Moderator.Controllers
{
    using Web.Controllers;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using static Common.CommonConstants;
    using static WebConstants;

    [Area(ModeratorArea)]
    [Authorize(Roles = ModeratorRole)]
    public class BaseModeratorController : Controller
    {
        protected string RedirectToModeratorReviewIndex()
        {
            return $"/{ModeratorRole}/{Reviews.ToLower()}/{nameof(ReviewsController.Index).ToLower()}";
        }

        protected IActionResult RedirectToHomeIndex()
        {
            return RedirectToAction(nameof(HomeController.Index), Home, new { area = string.Empty });
        }
    }
}