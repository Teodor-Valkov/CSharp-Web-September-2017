namespace FitStore.Web.Controllers
{
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Services.Contracts;
    using Services.Models.Manufacturers;
    using System.Threading.Tasks;

    using static Common.CommonConstants;
    using static Common.CommonMessages;

    public class ManufacturersController : BaseController
    {
        private IManufacturerService manufacturerService;

        public ManufacturersController(IManufacturerService manufacturerService)
        {
            this.manufacturerService = manufacturerService;
        }

        public async Task<IActionResult> Details(int id, string name)
        {
            bool isManufacturerExisting = await this.manufacturerService.IsManufacturerExistingById(id, false);

            if (!isManufacturerExisting)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, ManufacturerEntity));

                return RedirectToAction(nameof(HomeController.Index), Home);
            }

            ManufacturerDetailsServiceModel model = await this.manufacturerService.GetDetailsByIdAsync(id);

            ViewData["ReturnUrl"] = this.RedirectToManufacturerDetails(id, name);

            return View(model);
        }
    }
}