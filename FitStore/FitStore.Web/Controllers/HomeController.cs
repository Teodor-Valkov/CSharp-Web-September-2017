namespace FitStore.Web.Controllers
{
    using FitStore.Web.Models.Pagination;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Models.Home;
    using Services.Contracts;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using static Common.CommonConstants;

    public class HomeController : BaseController
    {
        private readonly ICategoryService categoryService;
        private readonly ISupplementService supplementService;

        public HomeController(ICategoryService categoryService, ISupplementService supplementService)
        {
            this.categoryService = categoryService;
            this.supplementService = supplementService;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            if (page < 1)
            {
                return RedirectToAction(nameof(Index));
            }

            PagingElementViewModel<HomeIndexViewModel> model = new PagingElementViewModel<HomeIndexViewModel>
            {
                Element = new HomeIndexViewModel
                {
                    Categories = await this.categoryService.GetAllAdvancedListingAsync(),
                    Supplements = await this.supplementService.GetAllAdvancedListingAsync(page)
                },
                Pagination = new PaginationViewModel
                {
                    TotalElements = await this.supplementService.TotalCountAsync(),
                    PageSize = HomePageSize,
                    CurrentPage = page
                }
            };

            if (page > model.Pagination.TotalPages && model.Pagination.TotalPages != 0)
            {
                return RedirectToAction(nameof(Index), new { page = model.Pagination.TotalPages });
            }

            ViewData["ReturnUrl"] = this.RedirectToHomeIndex();

            return View(model);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}