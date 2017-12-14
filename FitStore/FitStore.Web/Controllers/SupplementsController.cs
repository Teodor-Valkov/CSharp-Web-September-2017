namespace FitStore.Web.Controllers
{
    using FitStore.Web.Models.Pagination;
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Services.Contracts;
    using Services.Models.Supplements;
    using System.Threading.Tasks;

    using static Common.CommonConstants;
    using static Common.CommonMessages;

    public class SupplementsController : Controller
    {
        private readonly ISupplementService supplementService;

        public SupplementsController(ISupplementService supplementService)
        {
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

            PagingElementViewModel<SupplementDetailsServiceModel> model = new PagingElementViewModel<SupplementDetailsServiceModel>
            {
                Element = await this.supplementService.GetDetailsByIdAsync(id, page),
                Pagination = new PaginationViewModel
                {
                    TotalElements = await this.supplementService.TotalCommentsAsync(id, false),
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