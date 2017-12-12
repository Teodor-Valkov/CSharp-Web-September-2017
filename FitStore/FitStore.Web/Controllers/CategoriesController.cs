namespace FitStore.Web.Controllers
{
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Services.Contracts;
    using Services.Models.Categories;
    using System;
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

        public async Task<IActionResult> Details(int id, string name)
        {
            bool isCategoryExisting = await this.categoryService.IsCategoryExistingById(id, false);

            if (!isCategoryExisting)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, CategoryEntity));

                return RedirectToAction(nameof(HomeController.Index), Home);
            }

            CategoryDetailsServiceModel model = await this.categoryService.GetDetailsByIdAsync(id);

            ViewData["ReturnUrl"] = this.RedirectToCategoryDetails(id, name);

            return View(model);
        }
    }
}