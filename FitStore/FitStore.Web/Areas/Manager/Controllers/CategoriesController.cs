namespace FitStore.Web.Areas.Manager.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using Models.Categories;
    using Services.Manager.Contracts;
    using Services.Models.Categories;
    using System.Threading.Tasks;

    using static Common.CommonConstants;
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
                this.AddErrorMessage(string.Format(EntityExists, CategoryEntity));

                return View(model);
            }

            await this.managerCategoryService.CreateAsync(model.Name);

            this.AddSuccessMessage(string.Format(EntityCreated, CategoryEntity, model.Name));

            return this.RedirectToCategoriesIndex();
        }

        public async Task<IActionResult> Edit(int id, string name)
        {
            bool isCategoryExisting = await this.managerCategoryService.IsCategoryExistingById(id);

            if (!isCategoryExisting)
            {
                this.AddErrorMessage(string.Format(EntityNotFound, CategoryEntity, name));

                return this.RedirectToCategoriesIndex();
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
                this.AddErrorMessage(string.Format(EntityExists, CategoryEntity));

                return View(model);
            }

            await this.managerCategoryService.EditAsync(id, model.Name);

            this.AddSuccessMessage(string.Format(EntityEdited, CategoryEntity, model.Name));

            return this.RedirectToCategoriesIndex();
        }

        public async Task<IActionResult> Delete(int id, string name)
        {
            bool isCategoryExisting = await this.managerCategoryService.IsCategoryExistingById(id);

            if (!isCategoryExisting)
            {
                this.AddErrorMessage(string.Format(EntityNotFound, CategoryEntity, name));

                return this.RedirectToCategoriesIndex();
            }

            await this.managerCategoryService.DeleteAsync(id);

            this.AddSuccessMessage(string.Format(EntityDeleted, CategoryEntity, name));

            return this.RedirectToCategoriesIndex();
        }

        public async Task<IActionResult> Restore(int id, string name)
        {
            bool isCategoryExisting = await this.managerCategoryService.IsCategoryExistingById(id);

            if (!isCategoryExisting)
            {
                this.AddErrorMessage(string.Format(EntityNotFound, CategoryEntity, name));

                return this.RedirectToCategoriesIndex();
            }

            await this.managerCategoryService.RestoreAsync(id);

            this.AddSuccessMessage(string.Format(EntityRestored, CategoryEntity, name));

            return this.RedirectToCategoriesIndex();
        }
    }
}