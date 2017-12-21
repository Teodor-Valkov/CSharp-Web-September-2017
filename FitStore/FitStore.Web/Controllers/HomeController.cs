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

        public async Task<IActionResult> Index(string searchToken, int page = 1)
        {
            if (page < 1)
            {
                return RedirectToAction(nameof(Index), new { searchToken });
            }

            PagingElementViewModel<HomeIndexViewModel> model = new PagingElementViewModel<HomeIndexViewModel>
            {
                Element = new HomeIndexViewModel
                {
                    Categories = await this.categoryService.GetAllAdvancedListingAsync(),
                    Supplements = await this.supplementService.GetAllAdvancedListingAsync(searchToken, page)
                },
                SearchToken = searchToken,
                Pagination = new PaginationViewModel
                {
                    TotalElements = await this.supplementService.TotalCountAsync(searchToken),
                    PageSize = HomePageSize,
                    CurrentPage = page
                }
            };

            if (page > 1 && page > model.Pagination.TotalPages)
            {
                return RedirectToAction(nameof(Index), new { searchToken });
            }

            ViewData["SearchToken"] = searchToken;

            return View(model);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}