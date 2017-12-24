namespace FitStore.Web.Controllers
{
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Models.Pagination;
    using Services.Contracts;
    using Services.Models.Supplements;
    using System.Threading.Tasks;

    using static Common.CommonConstants;
    using static Common.CommonMessages;

    public class SupplementsController : BaseController
    {
        private readonly ISupplementService supplementService;

        public SupplementsController(ISupplementService supplementService)
        {
            this.supplementService = supplementService;
        }

        public async Task<IActionResult> Details(int id, string name, string returnUrl, int page = MinPage)
        {
            bool isSupplementExisting = await this.supplementService.IsSupplementExistingById(id, false);

            if (!isSupplementExisting)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, SupplementEntity));

                return this.RedirectToHomeIndex();
            }

            if (page < MinPage)
            {
                return RedirectToAction(nameof(Details), new { id, name });
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

            if (page > MinPage && page > model.Pagination.TotalPages)
            {
                return RedirectToAction(nameof(Details), new { id, name });
            }

            returnUrl = SetOrUpdateReturnUrl(returnUrl);

            return View(model);
        }

        private string SetOrUpdateReturnUrl(string returnUrl)
        {
            if (!Url.IsLocalUrl(returnUrl) && returnUrl != null)
            {
                returnUrl = this.ReturnToHomeIndex();
            }

            if (returnUrl == null)
            {
                returnUrl = TempData["ReturnUrl"].ToString();
            }

            TempData["ReturnUrl"] = returnUrl;
            ViewData["ReturnUrl"] = returnUrl;

            return returnUrl;
        }
    }
}