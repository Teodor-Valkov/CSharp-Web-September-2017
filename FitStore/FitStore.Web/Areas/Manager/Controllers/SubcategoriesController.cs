namespace FitStore.Web.Areas.Manager.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Models.Subcategories;
    using Services.Contracts;
    using Services.Manager.Contracts;
    using Services.Models.Categories;
    using Services.Models.Subcategories;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using static Common.CommonConstants;
    using static Common.CommonMessages;

    public class SubcategoriesController : BaseManagerController
    {
        private readonly IManagerSubcategoryService managerSubcategoryService;
        private readonly ISubcategoryService subcategoryService;
        private readonly ICategoryService categoryService;

        public SubcategoriesController(IManagerSubcategoryService managerSubcategoryService, ISubcategoryService subcategoryService, ICategoryService categoryService)
        {
            this.managerSubcategoryService = managerSubcategoryService;
            this.subcategoryService = subcategoryService;
            this.categoryService = categoryService;
        }

        public async Task<IActionResult> Create()
        {
            SubcategoryFormViewModel formModel = new SubcategoryFormViewModel
            {
                Categories = await this.GetCategoriesSelectListItems()
            };

            return View(formModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(SubcategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await this.GetCategoriesSelectListItems();

                return View(model);
            }

            bool isSubcategoryExisting = await this.subcategoryService.IsSubcategoryExistingByName(model.Name);

            if (isSubcategoryExisting)
            {
                this.AddErrorMessage(string.Format(EntityExists, SubcategoryEntity));

                return View(model);
            }

            await this.managerSubcategoryService.CreateAsync(model.Name, model.CategoryId);

            this.AddSuccessMessage(string.Format(EntityCreated, SubcategoryEntity, model.Name));

            return this.RedirectToSubcategoriesIndex();
        }

        public async Task<IActionResult> Edit(int id, string name)
        {
            bool isSubcategoryExisting = await this.subcategoryService.IsSubcategoryExistingById(id);

            if (!isSubcategoryExisting)
            {
                this.AddErrorMessage(string.Format(EntityNotFound, SubcategoryEntity, name));

                return this.RedirectToSubcategoriesIndex();
            }

            SubcategoryBasicServiceModel model = await this.managerSubcategoryService.GetEditModelAsync(id);

            SubcategoryFormViewModel formModel = Mapper.Map<SubcategoryFormViewModel>(model);

            formModel.Categories = await this.GetCategoriesSelectListItems();

            return View(formModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, SubcategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await this.GetCategoriesSelectListItems();

                return View(model);
            }

            bool isSubcategoryExisting = await this.subcategoryService.IsSubcategoryExistingByName(model.Name);

            if (isSubcategoryExisting)
            {
                this.AddErrorMessage(string.Format(EntityExists, SubcategoryEntity));

                return View(model);
            }

            await this.managerSubcategoryService.EditAsync(id, model.Name, model.CategoryId);

            this.AddSuccessMessage(string.Format(EntityEdited, SubcategoryEntity, model.Name));

            return this.RedirectToSubcategoriesIndex();
        }

        public async Task<IActionResult> Delete(int id, string name)
        {
            bool isSubcategoryExisting = await this.subcategoryService.IsSubcategoryExistingById(id);

            if (!isSubcategoryExisting)
            {
                this.AddErrorMessage(string.Format(EntityNotFound, SubcategoryEntity, name));

                return this.RedirectToSubcategoriesIndex();
            }

            await this.managerSubcategoryService.DeleteAsync(id);

            this.AddSuccessMessage(string.Format(EntityDeleted, SubcategoryEntity, name));

            return this.RedirectToSubcategoriesIndex();
        }

        public async Task<IActionResult> Restore(int id, string name)
        {
            bool isSubcategoryExisting = await this.subcategoryService.IsSubcategoryExistingById(id);

            if (!isSubcategoryExisting)
            {
                this.AddErrorMessage(string.Format(EntityNotFound, SubcategoryEntity, name));

                return this.RedirectToSubcategoriesIndex();
            }

            await this.managerSubcategoryService.RestoreAsync(id);

            this.AddSuccessMessage(string.Format(EntityRestored, SubcategoryEntity, name));

            return this.RedirectToSubcategoriesIndex();
        }

        private async Task<IEnumerable<SelectListItem>> GetCategoriesSelectListItems()
        {
            IEnumerable<CategoryBasicServiceModel> categories = await this.categoryService.GetAllBasicListingAsync();

            return categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            })
            .ToList();
        }
    }
}