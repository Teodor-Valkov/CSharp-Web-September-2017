namespace FitStore.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Models.Home;
    using Models.Pagination;
    using Services.Contracts;
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

        public async Task<IActionResult> Index(string searchToken, int page = MinPage)
        {
            if (page < MinPage)
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

            if (page > MinPage && page > model.Pagination.TotalPages)
            {
                return RedirectToAction(nameof(Index), new { searchToken });
            }

            ViewData["SearchToken"] = searchToken;

            return View(model);
        }
    }
}