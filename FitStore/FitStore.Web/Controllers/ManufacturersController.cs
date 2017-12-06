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

    public class ManufacturersController : Controller
    {
        private IManufacturerService manufacturerService;

        public ManufacturersController(IManufacturerService manufacturerService)
        {
            this.manufacturerService = manufacturerService;
        }

        public async Task<IActionResult> Index(string searchToken, int page = 1)
        {
            if (page < 1)
            {
                return RedirectToAction(nameof(Index));
            }

            PagingElementsViewModel<ManufacturerAdvancedServiceModel> model = new PagingElementsViewModel<ManufacturerAdvancedServiceModel>
            {
                Elements = await this.manufacturerService.GetAllListingAsync(searchToken, page),
                SearchToken = searchToken,
                Pagination = new PaginationViewModel
                {
                    TotalElements = await this.manufacturerService.TotalCountAsync(searchToken),
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

        public async Task<IActionResult> Details(int id, string name)
        {
            ManufacturerDetailsServiceModel model = await this.manufacturerService.GetDetailsByIdAsync(id);

            if (model == null)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, ManufacturerEntity, name));

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }
    }
}