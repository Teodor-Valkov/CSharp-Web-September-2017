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

        public async Task<IActionResult> Index(string searchToken, bool isDeleted, int page = 1)
        {
            if (page < 1)
            {
                return RedirectToAction(nameof(Index));
            }

            PagingElementsViewModel<CategoryAdvancedServiceModel> model = new PagingElementsViewModel<CategoryAdvancedServiceModel>
            {
                Elements = await this.managerCategoryService.GetAllPagedListingAsync(isDeleted, searchToken, page),
                SearchToken = searchToken,
                IsDeleted = isDeleted,
                Pagination = new PaginationViewModel
                {
                    TotalElements = await this.managerCategoryService.TotalCountAsync(isDeleted, searchToken),
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

            bool isCategoryExisting = await this.categoryService.IsCategoryExistingByName(model.Name);

            if (isCategoryExisting)
            {
                TempData.AddErrorMessage(string.Format(EntityExists, CategoryEntity));

                return View(model);
            }

            await this.managerCategoryService.CreateAsync(model.Name);

            TempData.AddSuccessMessage(string.Format(EntityCreated, CategoryEntity, model.Name));

            return this.RedirectToCategoriesIndex(false);
        }

        public async Task<IActionResult> Edit(int id, string name)
        {
            bool isCategoryExisting = await this.categoryService.IsCategoryExistingById(id);

            if (!isCategoryExisting)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, CategoryEntity));

                return this.RedirectToCategoriesIndex(false);
            }

            CategoryBasicServiceModel category = await this.managerCategoryService.GetEditModelAsync(id);

            CategoryFormViewModel model = Mapper.Map<CategoryFormViewModel>(category);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, string name, CategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (name != model.Name)
            {
                bool isCategoryExisting = await this.categoryService.IsCategoryExistingByName(model.Name);

                if (isCategoryExisting)
                {
                    TempData.AddErrorMessage(string.Format(EntityExists, CategoryEntity));

                    return View(model);
                }
            }

            await this.managerCategoryService.EditAsync(id, model.Name);

            TempData.AddSuccessMessage(string.Format(EntityEdited, CategoryEntity, model.Name));

            return this.RedirectToCategoriesIndex(false);
        }

        public async Task<IActionResult> Delete(int id, string name)
        {
            bool isCategoryExisting = await this.categoryService.IsCategoryExistingById(id);

            if (!isCategoryExisting)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, CategoryEntity));

                return this.RedirectToCategoriesIndex(false);
            }

            await this.managerCategoryService.DeleteAsync(id);

            TempData.AddSuccessMessage(string.Format(EntityDeleted, CategoryEntity, name));

            return this.RedirectToCategoriesIndex(false);
        }

        public async Task<IActionResult> Restore(int id, string name)
        {
            bool isCategoryExisting = await this.categoryService.IsCategoryExistingById(id);

            if (!isCategoryExisting)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, CategoryEntity));

                return this.RedirectToCategoriesIndex(true);
            }

            await this.managerCategoryService.RestoreAsync(id);

            TempData.AddSuccessMessage(string.Format(EntityRestored, CategoryEntity, name));

            return this.RedirectToCategoriesIndex(true);
        }
    }
}