namespace FitStore.Web.Areas.Manager.Controllers
{
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Services.Contracts;
    using Services.Manager.Contracts;
    using Services.Models.Manufacturers;
    using Services.Models.Supplements;
    using Services.Models.Subcategories;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

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

        public async Task<IActionResult> Create(int categoryId)
        {
            SupplementServiceModel formModel = new SupplementServiceModel();

            await PrepareModelToReturn(categoryId, formModel);

            return View(formModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(int categoryId, SupplementServiceModel model)
        {
            if (!ModelState.IsValid)
            {
                await PrepareModelToReturn(categoryId, model);

                return View(model);
            }

            bool isSupplementExisting = await this.supplementService.IsSupplementExistingByName(model.Name);

            if (isSupplementExisting)
            {
                this.AddErrorMessage(string.Format(EntityExists, SupplementEntity));

                await PrepareModelToReturn(categoryId, model);

                return View(model);
            }

            bool isSubcategoryExisting = await this.subcategoryService.IsSubcategoryExistingById(model.SubcategoryId);

            if (!isSubcategoryExisting)
            {
                this.AddErrorMessage(string.Format(EntityExists, SubcategoryEntity));

                await PrepareModelToReturn(categoryId, model);

                return View(model);
            }

            bool isManufacturerExisting = await this.manufacturerService.IsManufacturerExistingById(model.ManufacturerId);

            if (!isManufacturerExisting)
            {
                this.AddErrorMessage(string.Format(EntityExists, ManufacturerEntity));

                await PrepareModelToReturn(categoryId, model);

                return View(model);
            }

            byte[] picture = await model.Picture.ToByteArray();

            await this.managerSupplementService.CreateAsync(model.Name, model.Description, model.Quantity, model.Price, picture, model.BestBeforeDate, model.SubcategoryId, model.ManufacturerId);

            this.AddSuccessMessage(string.Format(EntityCreated, SupplementEntity, model.Name));

            return this.RedirectToSubcategoriesIndex();
        }

        public async Task<IActionResult> Edit(int categoryId, int id)
        {
            SupplementServiceModel formModel = await this.managerSupplementService.GetEditModelAsync(id);

            await PrepareModelToReturn(categoryId, formModel);

            return View(formModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int categoryId, int id, SupplementServiceModel model)
        {
            if (!ModelState.IsValid)
            {
                await PrepareModelToReturn(categoryId, model);

                return View(model);
            }

            bool isSupplementExisting = await this.supplementService.IsSupplementExistingByName(model.Name);

            if (isSupplementExisting)
            {
                this.AddErrorMessage(string.Format(EntityExists, SupplementEntity));

                await PrepareModelToReturn(categoryId, model);

                return View(model);
            }

            bool isSubcategoryExisting = await this.subcategoryService.IsSubcategoryExistingById(model.SubcategoryId);

            if (!isSubcategoryExisting)
            {
                this.AddErrorMessage(string.Format(EntityExists, SubcategoryEntity));

                await PrepareModelToReturn(categoryId, model);

                return View(model);
            }

            bool isManufacturerExisting = await this.manufacturerService.IsManufacturerExistingById(model.ManufacturerId);

            if (!isManufacturerExisting)
            {
                this.AddErrorMessage(string.Format(EntityExists, ManufacturerEntity));

                await PrepareModelToReturn(categoryId, model);

                return View(model);
            }

            byte[] picture = await model.Picture.ToByteArray();

            await this.managerSupplementService.CreateAsync(model.Name, model.Description, model.Quantity, model.Price, picture, model.BestBeforeDate, model.SubcategoryId, model.ManufacturerId);

            this.AddSuccessMessage(string.Format(EntityEdited, SupplementEntity, model.Name));

            return this.RedirectToSubcategoriesIndex();
        }

        public async Task<IActionResult> Delete(int id, string name)
        {
            bool isSupplementExisting = await this.supplementService.IsSupplementExistingById(id);

            if (!isSupplementExisting)
            {
                this.AddErrorMessage(string.Format(EntityNotFound, SupplementEntity, name));

                return this.RedirectToSubcategoriesIndex();
            }

            await this.managerSupplementService.DeleteAsync(id);

            this.AddSuccessMessage(string.Format(EntityDeleted, SupplementEntity, name));

            return this.RedirectToSubcategoriesIndex();
        }

        public async Task<IActionResult> Restore(int id, string name)
        {
            bool isSupplementExisting = await this.supplementService.IsSupplementExistingById(id);

            if (!isSupplementExisting)
            {
                this.AddErrorMessage(string.Format(EntityNotFound, SupplementEntity, name));

                return this.RedirectToSubcategoriesIndex();
            }

            await this.managerSupplementService.RestoreAsync(id);

            this.AddSuccessMessage(string.Format(EntityRestored, SupplementEntity, name));

            return this.RedirectToSubcategoriesIndex();
        }

        private async Task PrepareModelToReturn(int categoryId, SupplementServiceModel model)
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