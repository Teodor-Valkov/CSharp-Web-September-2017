﻿namespace FitStore.Web.Areas.Moderator.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Web.Controllers;

    using static Common.CommonConstants;
    using static WebConstants;

    [Area(ModeratorArea)]
    [Authorize(Roles = ModeratorRole)]
    public class BaseModeratorController : Controller
    {
        protected IActionResult RedirectToHomeIndex()
        {
            return RedirectToAction(nameof(HomeController.Index), Home);
        }

        protected string ReturnToHomeIndex()
        {
            return $"/{Home.ToLower()}/{nameof(HomeController.Index).ToLower()}";
        }
    }
}