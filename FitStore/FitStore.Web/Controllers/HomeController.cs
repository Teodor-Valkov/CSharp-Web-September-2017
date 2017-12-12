namespace FitStore.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Models.Home;
    using Services.Contracts;
    using System.Diagnostics;
    using System.Threading.Tasks;

    public class HomeController : BaseController
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

            ViewData["ReturnUrl"] = this.RedirectToHomeIndex();

            return View(model);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}