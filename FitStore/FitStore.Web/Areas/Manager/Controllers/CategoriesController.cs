namespace FitStore.Web.Areas.Manager.Controllers
{
    using AutoMapper;
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Models.Categories;
    using Services.Manager.Contracts;
    using Services.Models.Categories;
    using System.Threading.Tasks;

    using static Common.CommonMessages;

    public class CategoriesController : BaseManagerController
    {
        private readonly IManagerCategoryService managerCategoryService;

        public CategoriesController(IManagerCategoryService managerCategoryService)
        {
            this.managerCategoryService = managerCategoryService;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool isCategoryExisting = await this.managerCategoryService.IsCategoryExistingByName(model.Name);

            if (isCategoryExisting)
            {
                TempData.AddErrorMessage(CategoryExists);

                return View(model);
            }

            await this.managerCategoryService.CreateAsync(model.Name);

            TempData.AddSuccessMessage(string.Format(CategoryCreated, model.Name));

            return RedirectToAction(nameof(Web.Controllers.CategoriesController.Index), "Categories", new { area = "" });
        }

        public async Task<IActionResult> Edit(int id, string name)
        {
            bool isCategoryExisting = await this.managerCategoryService.IsCategoryExistingById(id);

            if (!isCategoryExisting)
            {
                TempData.AddErrorMessage(string.Format(CategoryNotFound, name));

                return RedirectToAction(nameof(Web.Controllers.CategoriesController.Index), "Categories", new { area = "" });
            }

            CategoryBasicServiceModel model = await this.managerCategoryService.GetEditModelAsync(id);

            CategoryFormViewModel formModel = Mapper.Map<CategoryFormViewModel>(model);

            return View(formModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, CategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool isCategoryExisting = await this.managerCategoryService.IsCategoryExistingByName(model.Name);

            if (isCategoryExisting)
            {
                TempData.AddErrorMessage(CategoryExists);

                return View(model);
            }

            await this.managerCategoryService.EditAsync(id, model.Name);

            TempData.AddSuccessMessage(string.Format(CategoryEdited, model.Name));

            return RedirectToAction(nameof(Web.Controllers.CategoriesController.Index), "Categories", new { area = "" });
        }

        public async Task<IActionResult> Delete(int id, string name)
        {
            bool isCategoryExisting = await this.managerCategoryService.IsCategoryExistingById(id);

            if (!isCategoryExisting)
            {
                TempData.AddErrorMessage(string.Format(CategoryNotFound, name));

                return RedirectToAction(nameof(Web.Controllers.CategoriesController.Index), "Categories", new { area = "" });
            }

            await this.managerCategoryService.DeleteAsync(id);

            TempData.AddSuccessMessage(string.Format(CategoryDeleted, name));

            return RedirectToAction(nameof(Web.Controllers.CategoriesController.Index), "Categories", new { area = "" });
        }

        public async Task<IActionResult> Restore(int id, string name)
        {
            bool isCategoryExisting = await this.managerCategoryService.IsCategoryExistingById(id);

            if (!isCategoryExisting)
            {
                TempData.AddErrorMessage(string.Format(CategoryNotFound, name));

                return RedirectToAction(nameof(Web.Controllers.CategoriesController.Index), "Categories", new { area = "" });
            }

            await this.managerCategoryService.RestoreAsync(id);

            TempData.AddSuccessMessage(string.Format(CategoryRestored, name));

            return RedirectToAction(nameof(Web.Controllers.CategoriesController.Index), "Categories", new { area = "" });
        }
    }
}