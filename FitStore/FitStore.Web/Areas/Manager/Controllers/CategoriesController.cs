namespace FitStore.Web.Areas.Manager.Controllers
{
    using AutoMapper;
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Models.Categories;
    using Services.Contracts;
    using Services.Manager.Contracts;
    using Services.Models.Categories;
    using System.Threading.Tasks;
    using Web.Models.Pagination;

    using static Common.CommonConstants;
    using static Common.CommonMessages;

    public class CategoriesController : BaseManagerController
    {
        private readonly IManagerCategoryService managerCategoryService;
        private readonly ICategoryService categoryService;

        public CategoriesController(IManagerCategoryService managerCategoryService, ICategoryService categoryService)
        {
            this.managerCategoryService = managerCategoryService;
            this.categoryService = categoryService;
        }

        public async Task<IActionResult> Index(string searchToken, bool isDeleted, int page = MinPage)
        {
            if (page < MinPage)
            {
                return RedirectToAction(nameof(Index), new { searchToken, isDeleted });
            }

            PagingElementsViewModel<CategoryAdvancedServiceModel> model = new PagingElementsViewModel<CategoryAdvancedServiceModel>
            {
                Elements = await this.managerCategoryService.GetAllPagedListingAsync(isDeleted, searchToken, page),
                SearchToken = searchToken,
                IsDeleted = isDeleted,
                Pagination = new PaginationViewModel
                {
                    TotalElements = await this.managerCategoryService.TotalCountAsync(isDeleted, searchToken),
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

            bool isCategoryExistingByName = await this.categoryService.IsCategoryExistingByName(model.Name);

            if (isCategoryExistingByName)
            {
                TempData.AddErrorMessage(string.Format(EntityExists, CategoryEntity));

                return View(model);
            }

            await this.managerCategoryService.CreateAsync(model.Name);

            TempData.AddSuccessMessage(string.Format(EntityCreated, CategoryEntity));

            return this.RedirectToCategoriesIndex(false);
        }

        public async Task<IActionResult> Edit(int id)
        {
            bool isCategoryExistingById = await this.categoryService.IsCategoryExistingById(id, false);

            if (!isCategoryExistingById)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, CategoryEntity));

                return this.RedirectToCategoriesIndex(false);
            }

            CategoryBasicServiceModel category = await this.managerCategoryService.GetEditModelAsync(id);

            CategoryFormViewModel model = Mapper.Map<CategoryFormViewModel>(category);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, CategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool isCategoryExistingById = await this.categoryService.IsCategoryExistingById(id, false);

            if (!isCategoryExistingById)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, CategoryEntity));

                return this.RedirectToCategoriesIndex(false);
            }

            bool isCategoryModified = await this.managerCategoryService.IsCategoryModified(id, model.Name);

            if (!isCategoryModified)
            {
                TempData.AddWarningMessage(EntityNotModified);

                return View(model);
            }

            bool isCategoryExistingByIdAndName = await this.categoryService.IsCategoryExistingByIdAndName(id, model.Name);

            if (isCategoryExistingByIdAndName)
            {
                TempData.AddErrorMessage(string.Format(EntityExists, CategoryEntity));

                return View(model);
            }

            await this.managerCategoryService.EditAsync(id, model.Name);

            TempData.AddSuccessMessage(string.Format(EntityModified, CategoryEntity));

            return this.RedirectToCategoriesIndex(false);
        }

        public async Task<IActionResult> Delete(int id)
        {
            bool isCategoryExistingById = await this.categoryService.IsCategoryExistingById(id, false);

            if (!isCategoryExistingById)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, CategoryEntity));

                return this.RedirectToCategoriesIndex(false);
            }

            await this.managerCategoryService.DeleteAsync(id);

            TempData.AddSuccessMessage(string.Format(EntityDeleted, CategoryEntity));

            return this.RedirectToCategoriesIndex(false);
        }

        public async Task<IActionResult> Restore(int id)
        {
            bool isCategoryExistingById = await this.categoryService.IsCategoryExistingById(id, true);

            if (!isCategoryExistingById)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, CategoryEntity));

                return this.RedirectToCategoriesIndex(true);
            }

            await this.managerCategoryService.RestoreAsync(id);

            TempData.AddSuccessMessage(string.Format(EntityRestored, CategoryEntity));

            return this.RedirectToCategoriesIndex(true);
        }
    }
}