namespace FitStore.Web.Areas.Manager.Controllers
{
    using Infrastructure.Extensions;
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
    using Web.Models.Pagination;

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

        public async Task<IActionResult> Index(string searchToken, bool isDeleted, int page = 1)
        {
            if (page < 1)
            {
                return RedirectToAction(nameof(Index));
            }

            PagingElementsViewModel<SubcategoryAdvancedServiceModel> model = new PagingElementsViewModel<SubcategoryAdvancedServiceModel>
            {
                Elements = await this.managerSubcategoryService.GetAllPagedListingAsync(isDeleted, searchToken, page),
                SearchToken = searchToken,
                IsDeleted = isDeleted,
                Pagination = new PaginationViewModel
                {
                    TotalElements = await this.managerSubcategoryService.TotalCountAsync(isDeleted, searchToken),
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

        public async Task<IActionResult> Create()
        {
            SubcategoryFormViewModel model = new SubcategoryFormViewModel
            {
                Categories = await this.GetCategoriesSelectListItems()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(SubcategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await this.GetCategoriesSelectListItems();

                return View(model);
            }

            bool isSubcategoryExistingByName = await this.subcategoryService.IsSubcategoryExistingByName(model.Name);

            if (isSubcategoryExistingByName)
            {
                TempData.AddErrorMessage(string.Format(EntityExists, SubcategoryEntity));

                model.Categories = await this.GetCategoriesSelectListItems();

                return View(model);
            }

            await this.managerSubcategoryService.CreateAsync(model.Name, model.CategoryId);

            TempData.AddSuccessMessage(string.Format(EntityCreated, SubcategoryEntity, model.Name));

            return this.RedirectToSubcategoriesIndex(false);
        }

        public async Task<IActionResult> Edit(int id)
        {
            bool isSubcategoryExistingById = await this.subcategoryService.IsSubcategoryExistingById(id);

            if (!isSubcategoryExistingById)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, SubcategoryEntity));

                return this.RedirectToSubcategoriesIndex(false);
            }

            SubcategoryBasicServiceModel subcategory = await this.managerSubcategoryService.GetEditModelAsync(id);

            SubcategoryFormViewModel model = new SubcategoryFormViewModel
            {
                Name = subcategory.Name,
                CategoryId = await this.subcategoryService.GetCategoryIdBySubcategoryId(subcategory.Id),
                Categories = await this.GetCategoriesSelectListItems()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, SubcategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await this.GetCategoriesSelectListItems();

                return View(model);
            }

            bool isSubcategoryExistingById = await this.subcategoryService.IsSubcategoryExistingById(id);

            if (!isSubcategoryExistingById)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, SubcategoryEntity));

                return this.RedirectToSubcategoriesIndex(false);
            }

            bool isSubcategoryModified = await this.managerSubcategoryService.IsSubcategoryModified(id, model.Name, model.CategoryId);

            if (!isSubcategoryModified)
            {
                TempData.AddWarningMessage(EntityNotModified);

                model.Categories = await this.GetCategoriesSelectListItems();

                return View(model);
            }

            bool isSubcategoryExistingByIdAndName = await this.subcategoryService.IsSubcategoryExistingByIdAndName(id, model.Name);

            if (isSubcategoryExistingByIdAndName)
            {
                TempData.AddErrorMessage(string.Format(EntityExists, SubcategoryEntity));

                model.Categories = await this.GetCategoriesSelectListItems();

                return View(model);
            }

            await this.managerSubcategoryService.EditAsync(id, model.Name, model.CategoryId);

            TempData.AddSuccessMessage(string.Format(EntityModified, SubcategoryEntity, model.Name));

            return this.RedirectToSubcategoriesIndex(false);
        }

        public async Task<IActionResult> Delete(int id, string name)
        {
            bool isSubcategoryExistingById = await this.subcategoryService.IsSubcategoryExistingById(id, false);

            if (!isSubcategoryExistingById)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, SubcategoryEntity));

                return this.RedirectToSubcategoriesIndex(false);
            }

            await this.managerSubcategoryService.DeleteAsync(id);

            TempData.AddSuccessMessage(string.Format(EntityDeleted, SubcategoryEntity, name));

            return this.RedirectToSubcategoriesIndex(false);
        }

        public async Task<IActionResult> Restore(int id, string name)
        {
            bool isSubcategoryExistingById = await this.subcategoryService.IsSubcategoryExistingById(id, true);

            if (!isSubcategoryExistingById)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, SubcategoryEntity));

                return this.RedirectToSubcategoriesIndex(true);
            }

            await this.managerSubcategoryService.RestoreAsync(id);

            TempData.AddSuccessMessage(string.Format(EntityRestored, SubcategoryEntity, name));

            return this.RedirectToSubcategoriesIndex(true);
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