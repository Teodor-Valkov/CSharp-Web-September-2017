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

    public class CategoriesController : Controller
    {
        private ICategoryService categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        public async Task<IActionResult> Index(string searchToken, int page = 1)
        {
            if (page < 1)
            {
                return RedirectToAction(nameof(Index));
            }

            PagingElementsViewModel<CategoryAdvancedServiceModel> model = new PagingElementsViewModel<CategoryAdvancedServiceModel>
            {
                Elements = await this.categoryService.GetAllListingAsync(searchToken, page),
                SearchToken = searchToken,
                Pagination = new PaginationViewModel
                {
                    TotalElements = await this.categoryService.TotalCountAsync(searchToken),
                    PageSize = CategoryPageSize,
                    CurrentPage = page
                }
            };

            if (page > model.Pagination.TotalPages && model.Pagination.TotalPages != 0)
            {
                return RedirectToAction(nameof(Index), new { page = model.Pagination.TotalPages });
            }

            return View(model);
        }

        public async Task<IActionResult> Details(int id, string name)
        {
            CategoryDetailsServiceModel model = await this.categoryService.GetDetailsByIdAsync(id);

            if (model == null)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, CategoryEntity, name));

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }
    }
}