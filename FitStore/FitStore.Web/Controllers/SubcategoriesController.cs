namespace FitStore.Web.Controllers
{
    using FitStore.Web.Models.Pagination;
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Services.Contracts;
    using Services.Models.Subcategories;
    using System.Threading.Tasks;

    using static Common.CommonConstants;
    using static Common.CommonMessages;

    public class SubcategoriesController : BaseController
    {
        private ISubcategoryService subcategoryService;

        public SubcategoriesController(ISubcategoryService subcategoryService)
        {
            this.subcategoryService = subcategoryService;
        }

        public async Task<IActionResult> Details(int id, string name, int page = 1)
        {
            bool isSubcategoryExisting = await this.subcategoryService.IsSubcategoryExistingById(id, false);

            if (!isSubcategoryExisting)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, SubcategoryEntity));

                return this.RedirectToHomeIndex();
            }

            if (page < 1)
            {
                return RedirectToAction(nameof(Details), new { id, name });
            }

            PagingElementViewModel<SubcategoryDetailsServiceModel> model = new PagingElementViewModel<SubcategoryDetailsServiceModel>
            {
                Element = await this.subcategoryService.GetDetailsByIdAsync(id, page),
                Pagination = new PaginationViewModel
                {
                    TotalElements = await this.subcategoryService.TotalSupplementsCountAsync(id),
                    PageSize = SupplementPageSize,
                    CurrentPage = page
                }
            };

            if (page > 1 && page > model.Pagination.TotalPages)
            {
                return RedirectToAction(nameof(Details), new { id, name });
            }

            ViewData["ReturnUrl"] = this.ReturnToSubcategoryDetails(id, name);

            return View(model);
        }
    }
}