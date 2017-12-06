namespace FitStore.Web.Controllers
{
    using FitStore.Services.Contracts;
    using FitStore.Web.Models.Home;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using System.Diagnostics;
    using System.Threading.Tasks;

    public class HomeController : Controller
    {
        private readonly ICategoryService categoryService;
        private readonly ISupplementService supplementService;

        public HomeController(ICategoryService categoryService, ISupplementService supplementService)
        {
            this.categoryService = categoryService;
            this.supplementService = supplementService;
        }

        public async Task<IActionResult> Index()
        {
            HomeIndexViewModel model = new HomeIndexViewModel()
            {
                Categories = await this.categoryService.GetAllAdvancedListingAsync(),
                Supplements = await this.supplementService.GetAllAdvancedListingAsync()
            };

            return View(model);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}