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

        public async Task<IActionResult> Index(string searchToken, bool isDeleted, int page = MinPage)
        {
            if (page < MinPage)
            {
                return RedirectToAction(nameof(Index), new { searchToken, isDeleted });
            }

            PagingElementsViewModel<ManufacturerAdvancedServiceModel> model = new PagingElementsViewModel<ManufacturerAdvancedServiceModel>
            {
                Elements = await this.managerManufacturerService.GetAllPagedListingAsync(isDeleted, searchToken, page),
                SearchToken = searchToken,
                IsDeleted = isDeleted,
                Pagination = new PaginationViewModel
                {
                    TotalElements = await this.managerManufacturerService.TotalCountAsync(isDeleted, searchToken),
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
        public async Task<IActionResult> Create(ManufacturerFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool isManufacturerExistingByName = await this.manufacturerService.IsManufacturerExistingByName(model.Name);

            if (isManufacturerExistingByName)
            {
                TempData.AddErrorMessage(string.Format(EntityExists, ManufacturerEntity));

                return View(model);
            }

            await this.managerManufacturerService.CreateAsync(model.Name, model.Address);

            TempData.AddSuccessMessage(string.Format(EntityCreated, ManufacturerEntity));

            return this.RedirectToManufacturersIndex(false);
        }

        public async Task<IActionResult> Edit(int id)
        {
            bool isManufacturerExistingById = await this.manufacturerService.IsManufacturerExistingById(id, false);

            if (!isManufacturerExistingById)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, ManufacturerEntity));

                return this.RedirectToManufacturersIndex(false);
            }

            ManufacturerBasicServiceModel manufacturer = await this.managerManufacturerService.GetEditModelAsync(id);

            ManufacturerFormViewModel model = Mapper.Map<ManufacturerFormViewModel>(manufacturer);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ManufacturerFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool isManufacturerExistingById = await this.manufacturerService.IsManufacturerExistingById(id, false);

            if (!isManufacturerExistingById)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, ManufacturerEntity));

                return this.RedirectToManufacturersIndex(false);
            }

            bool isManufacturerModified = await this.managerManufacturerService.IsManufacturerModified(id, model.Name, model.Address);

            if (!isManufacturerModified)
            {
                TempData.AddWarningMessage(EntityNotModified);

                return View(model);
            }

            bool isManufacturerExistingByIdAndName = await this.manufacturerService.IsManufacturerExistingByIdAndName(id, model.Name);

            if (isManufacturerExistingByIdAndName)
            {
                TempData.AddErrorMessage(string.Format(EntityExists, ManufacturerEntity));

                return View(model);
            }

            await this.managerManufacturerService.EditAsync(id, model.Name, model.Address);

            TempData.AddSuccessMessage(string.Format(EntityModified, ManufacturerEntity));

            return this.RedirectToManufacturersIndex(false);
        }

        public async Task<IActionResult> Delete(int id)
        {
            bool isManufacturerExistingById = await this.manufacturerService.IsManufacturerExistingById(id, false);

            if (!isManufacturerExistingById)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, ManufacturerEntity));

                return this.RedirectToManufacturersIndex(false);
            }

            await this.managerManufacturerService.DeleteAsync(id);

            TempData.AddSuccessMessage(string.Format(EntityDeleted, ManufacturerEntity));

            return this.RedirectToManufacturersIndex(false);
        }

        public async Task<IActionResult> Restore(int id)
        {
            bool isManufacturerExistingById = await this.manufacturerService.IsManufacturerExistingById(id, true);

            if (!isManufacturerExistingById)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, ManufacturerEntity));

                return this.RedirectToManufacturersIndex(true);
            }

            await this.managerManufacturerService.RestoreAsync(id);

            TempData.AddSuccessMessage(string.Format(EntityRestored, ManufacturerEntity));

            return this.RedirectToManufacturersIndex(true);
        }
    }
}