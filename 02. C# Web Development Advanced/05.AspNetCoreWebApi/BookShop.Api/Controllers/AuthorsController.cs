namespace BookShop.Api.Controllers
{
    using Infrastructure.Filters;
    using Models.Authors;
    using Services.Contracts;
    using Services.Models.Authors;
    using Services.Models.Books;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    public class AuthorsController : BaseController
    {
        private readonly IAuthorService authorService;

        public AuthorsController(IAuthorService authorService)
        {
            this.authorService = authorService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            AuthorDetailsServiceModel model = await this.authorService.GetAuthorDetailsByIdAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            return Ok(model);
        }

        [HttpGet("{id}/books")]
        public async Task<IActionResult> GetBooks(int id)
        {
            IEnumerable<BookWithCategoriesServiceModel> model = await this.authorService.GetBooksWithCategoriesByAuthorIdAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            return Ok(model);
        }

        [HttpPost]
        [ValidateModelState]
        public async Task<IActionResult> Post([FromBody]AuthorRequestPostModel model)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            int authorId = await this.authorService.CreateAsync(model.FirstName, model.LastName);

            return Ok(authorId);
        }
    }
}