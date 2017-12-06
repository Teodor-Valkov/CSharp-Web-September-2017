namespace FitStore.Web.Areas.Manager.Controllers
{
    using AutoMapper;
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Models.Supplements;
    using Services.Contracts;
    using Services.Manager.Contracts;
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
        private readonly ISupplementService supplementService;
        private readonly ISubcategoryService subcategoryService;
        private readonly IManufacturerService manufacturerService;

        public SupplementsController(IManagerSupplementService managerSupplementService, ISupplementService supplementService, ISubcategoryService subcategoryService, IManufacturerService manufacturerService)
        {
            this.managerSupplementService = managerSupplementService;
            this.supplementService = supplementService;
            this.subcategoryService = subcategoryService;
            this.manufacturerService = manufacturerService;
        }

        public async Task<IActionResult> Index(string searchToken, bool isDeleted, int page = 1)
        {
            if (page < 1)
            {
                return RedirectToAction(nameof(Index));
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

            if (page > model.Pagination.TotalPages && model.Pagination.TotalPages != 0)
            {
                return RedirectToAction(nameof(Index), new { page = model.Pagination.TotalPages });
            }

            return View(model);
        }

        public async Task<IActionResult> Create(int categoryId)
        {
            SupplementFormViewModel model = new SupplementFormViewModel
            {
                CategoryId = categoryId
            };

            await PrepareModelToReturn(model.CategoryId, model);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(SupplementFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PrepareModelToReturn(model.CategoryId, model);

                return View(model);
            }

            bool isSupplementExisting = await this.supplementService.IsSupplementExistingByName(model.Name);

            if (!isSupplementExisting)
            {
                TempData.AddErrorMessage(string.Format(EntityExists, SupplementEntity));

                await PrepareModelToReturn(model.CategoryId, model);

                return View(model);
            }

            bool isSubcategoryExisting = await this.subcategoryService.IsSubcategoryExistingById(model.SubcategoryId);

            if (!isSubcategoryExisting)
            {
                TempData.AddErrorMessage(string.Format(EntityExists, SubcategoryEntity));

                await PrepareModelToReturn(model.CategoryId, model);

                return View(model);
            }

            bool isManufacturerExisting = await this.manufacturerService.IsManufacturerExistingById(model.ManufacturerId);

            if (!isManufacturerExisting)
            {
                TempData.AddErrorMessage(string.Format(EntityExists, ManufacturerEntity));

                await PrepareModelToReturn(model.CategoryId, model);

                return View(model);
            }

            byte[] picture = await model.Picture.ToByteArray();

            await this.managerSupplementService.CreateAsync(model.Name, model.Description, model.Quantity, model.Price, picture, model.BestBeforeDate, model.SubcategoryId, model.ManufacturerId);

            TempData.AddSuccessMessage(string.Format(EntityCreated, SupplementEntity, model.Name));

            return this.RedirectToSupplementsIndex(false);
        }

        public async Task<IActionResult> Edit(int id, string name)
        {
            bool isSuplementExisting = await this.supplementService.IsSupplementExistingById(id);

            if (!isSuplementExisting)
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
                CategoryId = await this.subcategoryService.GetCategoryIdBySubcategoryId(supplement.SubcategoryId)
            };

            await PrepareModelToReturn(model.CategoryId, model);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, string name, SupplementFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PrepareModelToReturn(model.CategoryId, model);

                return View(model);
            }

            if (name != model.Name)
            {
                bool isSupplementExisting = await this.supplementService.IsSupplementExistingByName(model.Name);

                if (!isSupplementExisting)
                {
                    TempData.AddErrorMessage(string.Format(EntityExists, SupplementEntity));

                    await PrepareModelToReturn(model.CategoryId, model);

                    return View(model);
                }
            }

            bool isSubcategoryExisting = await this.subcategoryService.IsSubcategoryExistingById(model.SubcategoryId);

            if (!isSubcategoryExisting)
            {
                TempData.AddErrorMessage(string.Format(EntityExists, SubcategoryEntity));

                await PrepareModelToReturn(model.CategoryId, model);

                return View(model);
            }

            bool isManufacturerExisting = await this.manufacturerService.IsManufacturerExistingById(model.ManufacturerId);

            if (!isManufacturerExisting)
            {
                TempData.AddErrorMessage(string.Format(EntityExists, ManufacturerEntity));

                await PrepareModelToReturn(model.CategoryId, model);

                return View(model);
            }

            byte[] picture = await model.Picture.ToByteArray();

            await this.managerSupplementService.EditAsync(id, model.Name, model.Description, model.Quantity, model.Price, picture, model.BestBeforeDate, model.SubcategoryId, model.ManufacturerId);

            TempData.AddSuccessMessage(string.Format(EntityEdited, SupplementEntity, model.Name));

            return this.RedirectToSupplementsIndex(false);
        }

        public async Task<IActionResult> Delete(int id, string name)
        {
            bool isSupplementExisting = await this.supplementService.IsSupplementExistingById(id);

            if (!isSupplementExisting)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, SupplementEntity));

                return this.RedirectToSupplementsIndex(false);
            }

            await this.managerSupplementService.DeleteAsync(id);

            TempData.AddSuccessMessage(string.Format(EntityDeleted, SupplementEntity, name));

            return this.RedirectToSupplementsIndex(false);
        }

        public async Task<IActionResult> Restore(int id, string name)
        {
            bool isSupplementExisting = await this.supplementService.IsSupplementExistingById(id);

            if (!isSupplementExisting)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, SupplementEntity));

                return this.RedirectToSupplementsIndex(true);
            }

            await this.managerSupplementService.RestoreAsync(id);

            TempData.AddSuccessMessage(string.Format(EntityRestored, SupplementEntity, name));

            return this.RedirectToSupplementsIndex(true);
        }

        private async Task PrepareModelToReturn(int categoryId, SupplementFormViewModel model)
        {
            model.BestBeforeDate = DateTime.UtcNow;
            model.Subcategories = await this.GetSubcategoriesSelectListItems(categoryId);
            model.Manufacturers = await this.GetManufacturersSelectListItems();
        }

        private async Task<IEnumerable<SelectListItem>> GetSubcategoriesSelectListItems(int categoryId)
        {
            IEnumerable<SubcategoryBasicServiceModel> subcategories = await this.subcategoryService.GetAllBasicListingAsync(categoryId);

            return subcategories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            })
            .ToList();
        }

        private async Task<IEnumerable<SelectListItem>> GetManufacturersSelectListItems()
        {
            IEnumerable<ManufacturerBasicServiceModel> manufacturers = await this.manufacturerService.GetAllBasicListingAsync();

            return manufacturers.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            })
            .ToList();
        }
    }
}