namespace FitStore.Web.Controllers
{
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Models.Pagination;
    using Services.Contracts;
    using Services.Models.Categories;
    using System.Threading.Tasks;

    using static Common.CommonConstants;
    using static Common.CommonMessages;

    public class CategoriesController : BaseController
    {
        private ICategoryService categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        public async Task<IActionResult> Details(int id, string name, int page = MinPage)
        {
            bool isCategoryExisting = await this.categoryService.IsCategoryExistingById(id, false);

            if (!isCategoryExisting)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, CategoryEntity));

                return this.RedirectToHomeIndex();
            }

            if (page < MinPage)
            {
                return RedirectToAction(nameof(Details), new { id, name });
            }

            PagingElementViewModel<CategoryDetailsServiceModel> model = new PagingElementViewModel<CategoryDetailsServiceModel>
            {
                Element = await this.categoryService.GetDetailsByIdAsync(id, page),
                Pagination = new PaginationViewModel
                {
                    TotalElements = await this.categoryService.TotalSupplementsCountAsync(id),
                    PageSize = SupplementPageSize,
                    CurrentPage = page
                }
            };

            if (page > MinPage && page > model.Pagination.TotalPages)
            {
                return RedirectToAction(nameof(Details), new { id, name });
            }

            ViewData["ReturnUrl"] = this.ReturnToCategoryDetails(id, name);

            return View(model);
        }
    }
}