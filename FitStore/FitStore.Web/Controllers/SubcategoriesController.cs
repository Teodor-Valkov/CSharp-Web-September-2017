﻿namespace FitStore.Web.Controllers
{
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Mvc;
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

        public async Task<IActionResult> Details(int id, string name)
        {
            bool isSubcategoryExisting = await this.subcategoryService.IsSubcategoryExistingById(id);

            if (!isSubcategoryExisting)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, SubcategoryEntity));

                return RedirectToAction(nameof(HomeController.Index), Home);
            }

            SubcategoryDetailsServiceModel model = await this.subcategoryService.GetDetailsByIdAsync(id);

            return View(model);
        }
    }
}