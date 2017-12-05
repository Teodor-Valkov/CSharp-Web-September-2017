namespace LearningSystem.Web.Controllers
{
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Models.Home;
    using Services.Blog.Contracts;
    using Services.Contracts;
    using System.Diagnostics;
    using System.Threading.Tasks;

    public class HomeController : Controller
    {
        private readonly IUserService userService;
        private readonly ICourseService courseService;
        private readonly IBlogArticleService articleService;

        public HomeController(IUserService userService, ICourseService courseService, IBlogArticleService articleService)
        {
            this.userService = userService;
            this.courseService = courseService;
            this.articleService = articleService;
        }

        public IActionResult Index()
        {
            HomeIndexViewModel model = new HomeIndexViewModel();

            return View(model);
        }

        public async Task<IActionResult> Search(SearchViewModel model)
        {
            if (!model.SearchInUsers && !model.SearchInCourses && !model.SearchInArticles)
            {
                TempData.AddErrorMessage("Please select category to search.");

                return RedirectToAction(nameof(Index));
            }

            if (model.SearchInUsers)
            {
                model.Users = await this.userService.GetAllSearchListingAsync(model.SearchToken);
            }

            if (model.SearchInCourses)
            {
                model.Courses = await this.courseService.GetAllSearchListingAsync(model.SearchToken);
            }

            if (model.SearchInArticles)
            {
                model.Articles = await this.articleService.GetAllSearchListingAsync(model.SearchToken);
            }

            return View(model);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}