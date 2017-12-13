﻿namespace FitStore.Web.Areas.Moderator.Controllers
{
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Models.Pagination;
    using Services.Contracts;
    using Services.Moderator.Contracts;
    using Services.Moderator.Models.Supplements;
    using System.Threading.Tasks;
    using Web.Controllers;

    using static Common.CommonConstants;
    using static Common.CommonMessages;

    public class SupplementsController : BaseModeratorController
    {
        private readonly IModeratorSupplementService moderatorSupplementService;
        private readonly ISupplementService supplementService;

        public SupplementsController(IModeratorSupplementService moderatorSupplementService, ISupplementService supplementService)
        {
            this.moderatorSupplementService = moderatorSupplementService;
            this.supplementService = supplementService;
        }

        public async Task<IActionResult> Details(int id, string name, string returnUrl, int page = 1)
        {
            ViewData["ReturnUrl"] = returnUrl;

            bool isSupplementExisting = await this.supplementService.IsSupplementExistingById(id, false);

            if (!isSupplementExisting)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, SupplementEntity));

                return RedirectToAction(nameof(HomeController.Index), Home);
            }

            if (page < 1)
            {
                return RedirectToAction(nameof(Details), new { id });
            }

            PagingElementViewModel<SupplementDetailsWithDeletedCommentsServiceModel> model = new PagingElementViewModel<SupplementDetailsWithDeletedCommentsServiceModel>
            {
                Element = await this.moderatorSupplementService.GetDetailsWithDeletedCommentsByIdAsync(id, page),
                Pagination = new PaginationViewModel
                {
                    TotalElements = await this.supplementService.TotalCommentsAsync(id, true),
                    PageSize = CommentPageSize,
                    CurrentPage = page
                }
            };

            if (page > model.Pagination.TotalPages && model.Pagination.TotalPages != 0)
            {
                return RedirectToAction(nameof(Details), new { id, page = model.Pagination.TotalPages });
            }

            return View(model);
        }
    }
}