namespace FitStore.Web.Areas.Manager.Controllers
{
    using AutoMapper;
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Models.Manufacturers;
    using Services.Contracts;
    using Services.Manager.Contracts;
    using Services.Models.Manufacturers;
    using System.Threading.Tasks;
    using Web.Models.Pagination;

    using static Common.CommonConstants;
    using static Common.CommonMessages;

    public class ManufacturersController : BaseManagerController
    {
        private readonly IManagerManufacturerService managerManufacturerService;
        private readonly IManufacturerService manufacturerService;

        public ManufacturersController(IManagerManufacturerService managerManufacturerService, IManufacturerService manufacturerService)
        {
            this.managerManufacturerService = managerManufacturerService;
            this.manufacturerService = manufacturerService;
        }

        public async Task<IActionResult> Index(string searchToken, bool isDeleted, int page = 1)
        {
            if (page < 1)
            {
                return RedirectToAction(nameof(Index));
            }

            PagingElementsViewModel<ManufacturerAdvancedServiceModel> model = new PagingElementsViewModel<ManufacturerAdvancedServiceModel>
            {
                Elements = await this.managerManufacturerService.GetAllPagedListingAsync(isDeleted, searchToken, page),
                SearchToken = searchToken,
                IsDeleted = isDeleted,
                Pagination = new PaginationViewModel
                {
                    TotalElements = await this.managerManufacturerService.TotalCountAsync(isDeleted, searchToken),
                    PageSize = ManufacturerPageSize,
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
        public async Task<IActionResult> Create(ManufacturerFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool isManufacturerExisting = await this.manufacturerService.IsManufacturerExistingByName(model.Name);

            if (isManufacturerExisting)
            {
                TempData.AddErrorMessage(string.Format(EntityExists, ManufacturerEntity));

                return View(model);
            }

            await this.managerManufacturerService.CreateAsync(model.Name, model.Address);

            TempData.AddSuccessMessage(string.Format(EntityCreated, ManufacturerEntity, model.Name));

            return this.RedirectToManufacturersIndex(false);
        }

        public async Task<IActionResult> Edit(int id, string name)
        {
            bool isManufacturerExisting = await this.manufacturerService.IsManufacturerExistingById(id);

            if (!isManufacturerExisting)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, ManufacturerEntity));

                return this.RedirectToManufacturersIndex(false);
            }

            ManufacturerBasicServiceModel manufacturer = await this.managerManufacturerService.GetEditModelAsync(id);

            ManufacturerFormViewModel model = Mapper.Map<ManufacturerFormViewModel>(manufacturer);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, string name, ManufacturerFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (name != model.Name)
            {
                bool isManufacturerExisting = await this.manufacturerService.IsManufacturerExistingByName(model.Name);

                if (isManufacturerExisting)
                {
                    TempData.AddErrorMessage(string.Format(EntityExists, ManufacturerEntity));

                    return View(model);
                }
            }

            await this.managerManufacturerService.EditAsync(id, model.Name, model.Address);

            TempData.AddSuccessMessage(string.Format(EntityEdited, ManufacturerEntity, model.Name));

            return this.RedirectToManufacturersIndex(false);
        }

        public async Task<IActionResult> Delete(int id, string name)
        {
            bool isManufacturerExisting = await this.manufacturerService.IsManufacturerExistingById(id);

            if (!isManufacturerExisting)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, ManufacturerEntity));

                return this.RedirectToManufacturersIndex(false);
            }

            await this.managerManufacturerService.DeleteAsync(id);

            TempData.AddSuccessMessage(string.Format(EntityDeleted, ManufacturerEntity, name));

            return this.RedirectToManufacturersIndex(false);
        }

        public async Task<IActionResult> Restore(int id, string name)
        {
            bool isManufacturerExisting = await this.manufacturerService.IsManufacturerExistingById(id);

            if (!isManufacturerExisting)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, ManufacturerEntity));

                return this.RedirectToManufacturersIndex(true);
            }

            await this.managerManufacturerService.RestoreAsync(id);

            TempData.AddSuccessMessage(string.Format(EntityRestored, ManufacturerEntity, name));

            return this.RedirectToManufacturersIndex(true);
        }
    }
}