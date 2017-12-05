namespace FitStore.Web.Areas.Manager.Controllers
{
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Models.Manufacturers;
    using Services.Manager.Contracts;
    using Services.Models.Manufacturers;
    using System.Threading.Tasks;

    using static Common.CommonMessages;

    public class ManufacturersController : BaseManagerController
    {
        private readonly IManagerManufacturerService managerManufacturerService;

        public ManufacturersController(IManagerManufacturerService managerManufacturerService)
        {
            this.managerManufacturerService = managerManufacturerService;
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

            bool isManufacturerExisting = await this.managerManufacturerService.IsManufacturerExistingByName(model.Name);

            if (isManufacturerExisting)
            {
                TempData.AddErrorMessage(ManufacturerExists);

                return View(model);
            }

            await this.managerManufacturerService.CreateAsync(model.Name, model.Address);

            TempData.AddSuccessMessage(string.Format(ManufacturerCreated, model.Name));

            return RedirectToAction(nameof(Web.Controllers.ManufacturersController.Index), "Manufacturers", new { area = "" });
        }

        public async Task<IActionResult> Edit(int id, string name)
        {
            bool isManufacturerExisting = await this.managerManufacturerService.IsManufacturerExistingById(id);

            if (!isManufacturerExisting)
            {
                TempData.AddErrorMessage(string.Format(ManufacturerNotFound, name));

                return RedirectToAction(nameof(Web.Controllers.ManufacturersController.Index), "Manufacturers", new { area = "" });
            }

            ManufacturerBasicServiceModel model = await this.managerManufacturerService.GetEditModelAsync(id);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ManufacturerFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool isManufacturerExisting = await this.managerManufacturerService.IsManufacturerExistingByName(model.Name);

            if (isManufacturerExisting)
            {
                TempData.AddErrorMessage(ManufacturerExists);

                return View(model);
            }

            await this.managerManufacturerService.EditAsync(id, model.Name, model.Address);

            TempData.AddSuccessMessage(string.Format(ManufacturerEdited, model.Name));

            return RedirectToAction(nameof(Web.Controllers.ManufacturersController.Index), "Manufacturers", new { area = "" });
        }

        public async Task<IActionResult> Delete(int id, string name)
        {
            bool isManufacturerExisting = await this.managerManufacturerService.IsManufacturerExistingById(id);

            if (!isManufacturerExisting)
            {
                TempData.AddErrorMessage(string.Format(ManufacturerNotFound, name));

                return RedirectToAction(nameof(Web.Controllers.ManufacturersController.Index), "Manufacturers", new { area = "" });
            }

            await this.managerManufacturerService.DeleteAsync(id);

            TempData.AddSuccessMessage(string.Format(ManufacturerDeleted, name));

            return RedirectToAction(nameof(Web.Controllers.ManufacturersController.Index), "Manufacturers", new { area = "" });
        }

        public async Task<IActionResult> Restore(int id, string name)
        {
            bool isManufacturerExisting = await this.managerManufacturerService.IsManufacturerExistingById(id);

            if (!isManufacturerExisting)
            {
                TempData.AddErrorMessage(string.Format(ManufacturerNotFound, name));

                return RedirectToAction(nameof(Web.Controllers.ManufacturersController.Index), "Manufacturers", new { area = "" });
            }

            await this.managerManufacturerService.RestoreAsync(id);

            TempData.AddSuccessMessage(string.Format(ManufacturerRestored, name));

            return RedirectToAction(nameof(Web.Controllers.ManufacturersController.Index), "Manufacturers", new { area = "" });
        }
    }
}