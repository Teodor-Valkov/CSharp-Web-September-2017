namespace FitStore.Web.Areas.Manager.Controllers
{
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Models.Supplements;
    using Services.Contracts;
    using Services.Manager.Contracts;
    using Services.Models.Categories;
    using Services.Models.Manufacturers;
    using Services.Models.Supplements;
    using Services.Models.Subcategories;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Web.Models.Pagination;

    using static Common.CommonConstants;
    using static Common.CommonMessages;

    public class SupplementsController : BaseManagerController
    {
        private readonly IManagerSupplementService managerSupplementService;
        private readonly IManagerCategoryService managerCategoryService;
        private readonly IManagerSubcategoryService managerSubcategoryService;
        private readonly IManagerManufacturerService managerManufacturerService;
        private readonly ISupplementService supplementService;
        private readonly ICategoryService categoryService;
        private readonly ISubcategoryService subcategoryService;
        private readonly IManufacturerService manufacturerService;

        public SupplementsController(IManagerSupplementService managerSupplementService, IManagerCategoryService managerCategoryService, IManagerSubcategoryService managerSubcategoryService, IManagerManufacturerService managerManufacturerService, ISupplementService supplementService, ICategoryService categoryService, ISubcategoryService subcategoryService, IManufacturerService manufacturerService)
        {
            this.managerSupplementService = managerSupplementService;
            this.managerCategoryService = managerCategoryService;
            this.managerSubcategoryService = managerSubcategoryService;
            this.managerManufacturerService = managerManufacturerService;
            this.supplementService = supplementService;
            this.categoryService = categoryService;
            this.subcategoryService = subcategoryService;
            this.manufacturerService = manufacturerService;
        }

        public async Task<IActionResult> Index(string searchToken, bool isDeleted, int page = MinPage)
        {
            if (page < MinPage)
            {
                return RedirectToAction(nameof(Index), new { searchToken, isDeleted });
            }

            PagingElementsViewModel<SupplementAdvancedServiceModel> model = new PagingElementsViewModel<SupplementAdvancedServiceModel>
            {
                Elements = await this.managerSupplementService.GetAllPagedListingAsync(isDeleted, searchToken, page),
                SearchToken = searchToken,
                IsDeleted = isDeleted,
                Pagination = new PaginationViewModel
                {
                    TotalElements = await this.managerSupplementService.TotalCountAsync(isDeleted, searchToken),
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

        public async Task<IActionResult> ChooseCategory()
        {
            SupplementCategoryServiceModel model = new SupplementCategoryServiceModel
            {
                Categories = await this.GetCategoriesSelectListItems()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChooseCategory(SupplementCategoryServiceModel category)
        {
            if (!ModelState.IsValid)
            {
                category.Categories = await this.GetCategoriesSelectListItems();

                return View(nameof(ChooseCategory), category);
            }

            bool isCategoryExistingById = await this.categoryService.IsCategoryExistingById(category.CategoryId, false);

            if (!isCategoryExistingById)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, CategoryEntity));

                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Create), new { category.CategoryId });
        }

        public async Task<IActionResult> Create(int categoryId)
        {
            SupplementFormViewModel model = new SupplementFormViewModel();

            await PrepareModelToReturn(categoryId, model);

            ViewData["CategoryId"] = categoryId;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> FinishCreate(int categoryId, SupplementFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PrepareModelToReturn(categoryId, model);

                ViewData["CategoryId"] = categoryId;

                return View(nameof(Create), model);
            }

            bool isSupplementExistingByName = await this.supplementService.IsSupplementExistingByName(model.Name);

            if (isSupplementExistingByName)
            {
                TempData.AddErrorMessage(string.Format(EntityExists, SupplementEntity));

                await PrepareModelToReturn(categoryId, model);

                ViewData["CategoryId"] = categoryId;

                return View(nameof(Create), model);
            }

            bool isSubcategoryExistingById = await this.subcategoryService.IsSubcategoryExistingById(model.SubcategoryId, false);

            if (!isSubcategoryExistingById)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, SubcategoryEntity));

                await PrepareModelToReturn(categoryId, model);

                return View(model);
            }

            bool isManufacturerExistingById = await this.manufacturerService.IsManufacturerExistingById(model.ManufacturerId, false);

            if (!isManufacturerExistingById)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, ManufacturerEntity));

                await PrepareModelToReturn(categoryId, model);

                return View(model);
            }

            byte[] picture = await model.Picture.ToByteArray();

            await this.managerSupplementService.CreateAsync(model.Name, model.Description, model.Quantity, model.Price, picture, model.BestBeforeDate, model.SubcategoryId, model.ManufacturerId);

            TempData.AddSuccessMessage(string.Format(EntityCreated, SupplementEntity));

            return this.RedirectToSupplementsIndex(false);
        }

        public async Task<IActionResult> Edit(int id)
        {
            bool isSuplementExistingById = await this.supplementService.IsSupplementExistingById(id, false);

            if (!isSuplementExistingById)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, SupplementEntity));

                return this.RedirectToSupplementsIndex(false);
            }

            SupplementServiceModel supplement = await this.managerSupplementService.GetEditModelAsync(id);

            SupplementFormViewModel model = new SupplementFormViewModel()
            {
                Name = supplement.Name,
                Description = supplement.Description,
                Quantity = supplement.Quantity,
                Price = supplement.Price,
                BestBeforeDate = supplement.BestBeforeDate,

                SubcategoryId = supplement.SubcategoryId,
                ManufacturerId = supplement.ManufacturerId,
            };

            int categoryId = await this.subcategoryService.GetCategoryIdBySubcategoryId(supplement.SubcategoryId);

            await PrepareModelToReturn(categoryId, model);

            ViewData["CategoryId"] = categoryId;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, int categoryId, SupplementFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PrepareModelToReturn(categoryId, model);

                ViewData["CategoryId"] = categoryId;

                return View(model);
            }

            bool isSuplementExistingById = await this.supplementService.IsSupplementExistingById(id, false);

            if (!isSuplementExistingById)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, SupplementEntity));

                return this.RedirectToSupplementsIndex(false);
            }

            byte[] picture = await model.Picture.ToByteArray();

            bool isSupplementModified = await this.managerSupplementService.IsSupplementModified(id, model.Name, model.Description, model.Quantity, model.Price, picture, model.BestBeforeDate, model.SubcategoryId, model.ManufacturerId);

            if (!isSupplementModified)
            {
                TempData.AddWarningMessage(EntityNotModified);

                await PrepareModelToReturn(categoryId, model);

                return View(model);
            }

            bool isSupplementExistingByIdAndName = await this.supplementService.IsSupplementExistingByIdAndName(id, model.Name);

            if (isSupplementExistingByIdAndName)
            {
                TempData.AddErrorMessage(string.Format(EntityExists, SupplementEntity));

                await PrepareModelToReturn(categoryId, model);

                return View(model);
            }

            bool isSubcategoryExistingById = await this.subcategoryService.IsSubcategoryExistingById(model.SubcategoryId, false);

            if (!isSubcategoryExistingById)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, SubcategoryEntity));

                await PrepareModelToReturn(categoryId, model);

                return View(model);
            }

            bool isManufacturerExisting = await this.manufacturerService.IsManufacturerExistingById(model.ManufacturerId, false);

            if (!isManufacturerExisting)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, ManufacturerEntity));

                await PrepareModelToReturn(categoryId, model);

                return View(model);
            }

            await this.managerSupplementService.EditAsync(id, model.Name, model.Description, model.Quantity, model.Price, picture, model.BestBeforeDate, model.SubcategoryId, model.ManufacturerId);

            TempData.AddSuccessMessage(string.Format(EntityModified, SupplementEntity));

            return this.RedirectToSupplementsIndex(false);
        }

        public async Task<IActionResult> Delete(int id)
        {
            bool isSupplementExistingById = await this.supplementService.IsSupplementExistingById(id, false);

            if (!isSupplementExistingById)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, SupplementEntity));

                return this.RedirectToSupplementsIndex(false);
            }

            await this.managerSupplementService.DeleteAsync(id);

            TempData.AddSuccessMessage(string.Format(EntityDeleted, SupplementEntity));

            return this.RedirectToSupplementsIndex(false);
        }

        public async Task<IActionResult> Restore(int id)
        {
            bool isSupplementExistingById = await this.supplementService.IsSupplementExistingById(id, true);

            if (!isSupplementExistingById)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, SupplementEntity));

                return this.RedirectToSupplementsIndex(true);
            }

            string restoreResult = await this.managerSupplementService.RestoreAsync(id);

            if (restoreResult == string.Empty)
            {
                TempData.AddSuccessMessage(string.Format(EntityRestored, SupplementEntity));
            }
            else
            {
                TempData.AddErrorMessage(string.Format(EntityNotRestored, SupplementEntity) + restoreResult);
            }

            return this.RedirectToSupplementsIndex(true);
        }

        private async Task PrepareModelToReturn(int categoryId, SupplementFormViewModel model)
        {
            model.BestBeforeDate = DateTime.UtcNow.AddDays(1);
            model.Subcategories = await this.GetSubcategoriesSelectListItems(categoryId);
            model.Manufacturers = await this.GetManufacturersSelectListItems();
        }

        private async Task<IEnumerable<SelectListItem>> GetCategoriesSelectListItems()
        {
            IEnumerable<CategoryBasicServiceModel> categories = await this.managerCategoryService.GetAllBasicListingWithAnySubcategoriesAsync(false);

            return categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            })
            .ToList();
        }

        private async Task<IEnumerable<SelectListItem>> GetSubcategoriesSelectListItems(int categoryId)
        {
            IEnumerable<SubcategoryBasicServiceModel> subcategories = await this.managerSubcategoryService.GetAllBasicListingAsync(categoryId, false);

            return subcategories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            })
            .ToList();
        }

        private async Task<IEnumerable<SelectListItem>> GetManufacturersSelectListItems()
        {
            IEnumerable<ManufacturerBasicServiceModel> manufacturers = await this.managerManufacturerService.GetAllBasicListingAsync(false);

            return manufacturers.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            })
            .ToList();
        }
    }
}