namespace LearningSystem.Web.Areas.Blog.Controllers
{
    using Data.Models;
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Models.Articles;
    using Services.Blog.Contracts;
    using Services.Blog.Models.Articles;
    using Services.Contracts;
    using System.Threading.Tasks;
    using Web.Models;

    using static Common.CommonConstants;
    using static WebConstants;

    [Area(BlogArea)]
    [Authorize(Roles = BlogAuthorRole)]
    public class ArticlesController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly IBlogArticleService articleService;
        private readonly IHtmlService htmlService;

        public ArticlesController(UserManager<User> userManager, IBlogArticleService articleService, IHtmlService htmlService)
        {
            this.userManager = userManager;
            this.articleService = articleService;
            this.htmlService = htmlService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(string searchToken = null, int page = 1)
        {
            if (page < 1)
            {
                return RedirectToAction(nameof(Index));
            }

            PagesViewModel<ArticleBasicServiceModel> model = new PagesViewModel<ArticleBasicServiceModel>
            {
                Elements = await this.articleService.GetAllListingAsync(searchToken, page),
                SearchToken = searchToken,
                Pagination = new PaginationViewModel
                {
                    TotalElements = await this.articleService.TotalCountAsync(searchToken),
                    PageSize = ArticlePageSize,
                    CurrentPage = page
                }
            };

            if (page > model.Pagination.TotalPages && model.Pagination.TotalPages != 0)
            {
                return RedirectToAction(nameof(Index), new { page = model.Pagination.TotalPages });
            }

            return View(model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            ArticleDetailsServiceModel model = await this.articleService.GetDetailsByIdAsync(id);

            return this.ViewOrNotFound(model);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ArticleFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string userId = this.userManager.GetUserId(User);
            model.Content = this.htmlService.Sanitize(model.Content);

            await this.articleService.CreateAsync(model.Title, model.Content, model.PublishDate, userId);

            TempData.AddSuccessMessage($"Article '{model.Title}' has been successfully created.");
            return RedirectToAction(nameof(Index));
        }
    }
}