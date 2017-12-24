namespace FitStore.Web.Controllers
{
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Models.Pagination;
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

        public async Task<IActionResult> Details(int id, string name, int page = MinPage)
        {
            bool isSubcategoryExisting = await this.subcategoryService.IsSubcategoryExistingById(id, false);

            if (!isSubcategoryExisting)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, SubcategoryEntity));

                return this.RedirectToHomeIndex();
            }

            if (page < MinPage)
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

            if (page > MinPage && page > model.Pagination.TotalPages)
            {
                return RedirectToAction(nameof(Details), new { id, name });
            }

            ViewData["ReturnUrl"] = this.ReturnToSubcategoryDetails(id, name);

            return View(model);
        }
    }
}