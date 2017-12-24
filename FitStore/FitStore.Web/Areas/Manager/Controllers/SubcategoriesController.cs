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
        private readonly IManagerCategoryService managerCategoryService;
        private readonly ISubcategoryService subcategoryService;

        public SubcategoriesController(IManagerSubcategoryService managerSubcategoryService, IManagerCategoryService managerCategoryService, ISubcategoryService subcategoryService)
        {
            this.managerSubcategoryService = managerSubcategoryService;
            this.managerCategoryService = managerCategoryService;
            this.subcategoryService = subcategoryService;
        }

        public async Task<IActionResult> Index(string searchToken, bool isDeleted, int page = MinPage)
        {
            if (page < MinPage)
            {
                return RedirectToAction(nameof(Index), new { searchToken, isDeleted });
            }

            PagingElementsViewModel<SubcategoryAdvancedServiceModel> model = new PagingElementsViewModel<SubcategoryAdvancedServiceModel>
            {
                Elements = await this.managerSubcategoryService.GetAllPagedListingAsync(isDeleted, searchToken, page),
                SearchToken = searchToken,
                IsDeleted = isDeleted,
                Pagination = new PaginationViewModel
                {
                    TotalElements = await this.managerSubcategoryService.TotalCountAsync(isDeleted, searchToken),
                    PageSize = SupplementPageSize,
                    CurrentPage = page
                }
            };

            if (page > MinPage && page > model.Pagination.TotalPages)
            {
                return RedirectToAction(nameof(Index), new { searchToken, isDeleted });
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

            TempData.AddSuccessMessage(string.Format(EntityCreated, SubcategoryEntity));

            return this.RedirectToSubcategoriesIndex(false);
        }

        public async Task<IActionResult> Edit(int id)
        {
            bool isSubcategoryExistingById = await this.subcategoryService.IsSubcategoryExistingById(id, false);

            if (!isSubcategoryExistingById)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, SubcategoryEntity));

                return this.RedirectToSubcategoriesIndex(false);
            }

            SubcategoryBasicServiceModel subcategory = await this.managerSubcategoryService.GetEditModelAsync(id);

            SubcategoryFormViewModel model = new SubcategoryFormViewModel
            {
                Name = subcategory.Name,
                CategoryId = await this.subcategoryService.GetCategoryIdBySubcategoryId(id),
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

            bool isSubcategoryExistingById = await this.subcategoryService.IsSubcategoryExistingById(id, false);

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

            TempData.AddSuccessMessage(string.Format(EntityModified, SubcategoryEntity));

            return this.RedirectToSubcategoriesIndex(false);
        }

        public async Task<IActionResult> Delete(int id)
        {
            bool isSubcategoryExistingById = await this.subcategoryService.IsSubcategoryExistingById(id, false);

            if (!isSubcategoryExistingById)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, SubcategoryEntity));

                return this.RedirectToSubcategoriesIndex(false);
            }

            await this.managerSubcategoryService.DeleteAsync(id);

            TempData.AddSuccessMessage(string.Format(EntityDeleted, SubcategoryEntity));

            return this.RedirectToSubcategoriesIndex(false);
        }

        public async Task<IActionResult> Restore(int id)
        {
            bool isSubcategoryExistingById = await this.subcategoryService.IsSubcategoryExistingById(id, true);

            if (!isSubcategoryExistingById)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, SubcategoryEntity));

                return this.RedirectToSubcategoriesIndex(true);
            }

            string restoreResult = await this.managerSubcategoryService.RestoreAsync(id);

            if (restoreResult == string.Empty)
            {
                TempData.AddSuccessMessage(string.Format(EntityRestored, SubcategoryEntity));
            }
            else
            {
                TempData.AddErrorMessage(string.Format(EntityNotRestored, SubcategoryEntity) + restoreResult);
            }

            return this.RedirectToSubcategoriesIndex(true);
        }

        private async Task<IEnumerable<SelectListItem>> GetCategoriesSelectListItems()
        {
            IEnumerable<CategoryBasicServiceModel> categories = await this.managerCategoryService.GetAllBasicListingAsync(false);

            return categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            })
            .ToList();
        }
    }
}