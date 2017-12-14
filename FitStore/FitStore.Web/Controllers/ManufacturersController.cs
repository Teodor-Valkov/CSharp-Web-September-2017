namespace FitStore.Web.Controllers
{
    using FitStore.Web.Models.Pagination;
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

        public async Task<IActionResult> Index(int page = 1)
        {
            if (page < 1)
            {
                return RedirectToAction(nameof(Index));
            }

            PagingElementsViewModel<ManufacturerAdvancedServiceModel> model = new PagingElementsViewModel<ManufacturerAdvancedServiceModel>
            {
                Elements = await this.manufacturerService.GetAllPagedListingAsync(page),
                Pagination = new PaginationViewModel
                {
                    TotalElements = await this.manufacturerService.TotalCountAsync(),
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

        public async Task<IActionResult> Details(int id, string name, int page = 1)
        {
            bool isManufacturerExisting = await this.manufacturerService.IsManufacturerExistingById(id, false);

            if (!isManufacturerExisting)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, ManufacturerEntity));

                return RedirectToAction(nameof(HomeController.Index), Home);
            }

            if (page < 1)
            {
                return RedirectToAction(nameof(Details), new { id, name });
            }

            PagingElementViewModel<ManufacturerDetailsServiceModel> model = new PagingElementViewModel<ManufacturerDetailsServiceModel>
            {
                Element = await this.manufacturerService.GetDetailsByIdAsync(id, page),
                Pagination = new PaginationViewModel
                {
                    TotalElements = await this.manufacturerService.TotalSupplementsCountAsync(id),
                    PageSize = SupplementPageSize,
                    CurrentPage = page
                }
            };

            if (page > model.Pagination.TotalPages && model.Pagination.TotalPages != 0)
            {
                return RedirectToAction(nameof(Details), new { id, name, page = model.Pagination.TotalPages });
            }

            ViewData["ReturnUrl"] = this.RedirectToManufacturerDetails(id, name);

            return View(model);
        }
    }
}