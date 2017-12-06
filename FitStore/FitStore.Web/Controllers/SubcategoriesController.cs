namespace FitStore.Web.Controllers
{
    using FitStore.Web.Models.Subcategories;
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Models.Pagination;
    using Services.Contracts;
    using Services.Models.Subcategories;
    using System.Threading.Tasks;

    using static Common.CommonConstants;
    using static Common.CommonMessages;

    public class SubcategoriesController : Controller
    {
        private ISubcategoryService subcategoryService;

        public SubcategoriesController(ISubcategoryService subcategoryService)
        {
            this.subcategoryService = subcategoryService;
        }

        public async Task<IActionResult> Index(string searchToken, int page = 1)
        {
            if (page < 1)
            {
                return RedirectToAction(nameof(Index));
            }

            SubcategoryPageViewModel model = new SubcategoryPageViewModel
            {
                Elements = await this.subcategoryService.GetAllListingAsync(searchToken, page),
                SearchToken = searchToken,
                Pagination = new PaginationViewModel
                {
                    TotalElements = await this.subcategoryService.TotalCountAsync(searchToken),
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
            SubcategoryDetailsServiceModel model = await this.subcategoryService.GetDetailsByIdAsync(id);

            if (model == null)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, SubcategoryEntity, name));

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }
    }
}