namespace FitStore.Web.Controllers
{
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Models.Pagination;
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

        public async Task<IActionResult> Index(int page = MinPage)
        {
            if (page < MinPage)
            {
                return RedirectToAction(nameof(Index));
            }

            PagingElementsViewModel<ManufacturerAdvancedServiceModel> model = new PagingElementsViewModel<ManufacturerAdvancedServiceModel>
            {
                Elements = await this.manufacturerService.GetAllPagedListingAsync(page),
                Pagination = new PaginationViewModel
                {
                    TotalElements = await this.manufacturerService.TotalCountAsync(),
                    PageSize = ManufacturerPageSize,
                    CurrentPage = page
                }
            };

            if (page > MinPage && page > model.Pagination.TotalPages)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        public async Task<IActionResult> Details(int id, string name, int page = MinPage)
        {
            bool isManufacturerExisting = await this.manufacturerService.IsManufacturerExistingById(id, false);

            if (!isManufacturerExisting)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, ManufacturerEntity));

                return this.RedirectToHomeIndex();
            }

            if (page < MinPage)
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

            if (page > MinPage && page > model.Pagination.TotalPages)
            {
                return RedirectToAction(nameof(Details), new { id, name });
            }

            ViewData["ReturnUrl"] = this.ReturnToManufacturerDetails(id, name);

            return View(model);
        }
    }
}