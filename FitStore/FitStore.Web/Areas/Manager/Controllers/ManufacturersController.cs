namespace FitStore.Web.Areas.Manager.Controllers
{
    using FitStore.Services.Contracts;
    using Microsoft.AspNetCore.Mvc;
    using Models.Manufacturers;
    using Services.Manager.Contracts;
    using Services.Models.Manufacturers;
    using System.Threading.Tasks;

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
                this.AddErrorMessage(string.Format(EntityExists, ManufacturerEntity));

                return View(model);
            }

            await this.managerManufacturerService.CreateAsync(model.Name, model.Address);

            this.AddSuccessMessage(string.Format(EntityCreated, ManufacturerEntity, model.Name));

            return this.RedirectToManufacturersIndex();
        }

        public async Task<IActionResult> Edit(int id, string name)
        {
            bool isManufacturerExisting = await this.manufacturerService.IsManufacturerExistingById(id);

            if (!isManufacturerExisting)
            {
                this.AddErrorMessage(string.Format(EntityNotFound, ManufacturerEntity, name));

                return this.RedirectToManufacturersIndex();
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

            bool isManufacturerExisting = await this.manufacturerService.IsManufacturerExistingByName(model.Name);

            if (isManufacturerExisting)
            {
                this.AddErrorMessage(string.Format(EntityExists, ManufacturerEntity));

                return View(model);
            }

            await this.managerManufacturerService.EditAsync(id, model.Name, model.Address);

            this.AddSuccessMessage(string.Format(EntityEdited, ManufacturerEntity, model.Name));

            return this.RedirectToManufacturersIndex();
        }

        public async Task<IActionResult> Delete(int id, string name)
        {
            bool isManufacturerExisting = await this.manufacturerService.IsManufacturerExistingById(id);

            if (!isManufacturerExisting)
            {
                this.AddErrorMessage(string.Format(EntityNotFound, ManufacturerEntity, name));

                return this.RedirectToManufacturersIndex();
            }

            await this.managerManufacturerService.DeleteAsync(id);

            this.AddSuccessMessage(string.Format(EntityDeleted, ManufacturerEntity, name));

            return this.RedirectToManufacturersIndex();
        }

        public async Task<IActionResult> Restore(int id, string name)
        {
            bool isManufacturerExisting = await this.manufacturerService.IsManufacturerExistingById(id);

            if (!isManufacturerExisting)
            {
                this.AddErrorMessage(string.Format(EntityNotFound, ManufacturerEntity, name));

                return this.RedirectToManufacturersIndex();
            }

            await this.managerManufacturerService.RestoreAsync(id);

            this.AddSuccessMessage(string.Format(EntityRestored, ManufacturerEntity, name));

            return this.RedirectToManufacturersIndex();
        }
    }
}