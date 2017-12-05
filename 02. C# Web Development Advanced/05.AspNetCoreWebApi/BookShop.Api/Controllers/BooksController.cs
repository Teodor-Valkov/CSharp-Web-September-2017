namespace BookShop.Api.Controllers
{
    using Infrastructure.Filters;
    using Microsoft.AspNetCore.Mvc;
    using Models.Books;
    using Services.Contracts;
    using Services.Models.Books;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class BooksController : BaseController
    {
        private readonly IAuthorService authorService;
        private readonly IBookService bookService;

        public BooksController(IAuthorService authorService, IBookService bookService)
        {
            this.authorService = authorService;
            this.bookService = bookService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]string search = "")
        {
            IEnumerable<BookBasicServiceModel> model = await this.bookService.GetAllListingAsync(search);

            if (model == null)
            {
                return NotFound();
            }

            return Ok(model);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            BookDetailsServiceModel model = await this.bookService.GetBookDetailsByIdAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            return Ok(model);
        }

        [HttpPost]
        [ValidateModelState]
        public async Task<IActionResult> Post([FromBody] BookRequestPostModel model)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            bool isAuthorExisting = await this.authorService.IsAuthorExistingAsync(model.AuthorId);

            if (!isAuthorExisting)
            {
                return BadRequest("Author does not exist.");
            }

            int bookId = await this.bookService.CreateAsync(
                model.Title,
                model.Description,
                model.Price,
                model.Copies,
                model.Edition,
                model.AgeRestriction,
                model.ReleaseDate,
                model.AuthorId,
                model.Categories);

            return Ok(bookId);
        }

        [HttpPut("{id}")]
        [ValidateModelState]
        public async Task<IActionResult> Put(int id, [FromBody]BookRequestPostModel model)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            bool isAuthorExisting = await this.authorService.IsAuthorExistingAsync(model.AuthorId);

            if (!isAuthorExisting)
            {
                return BadRequest("Author does not exist.");
            }

            int bookId = await this.bookService.EditAsync(
                id,
                model.Title,
                model.Description,
                model.Price,
                model.Copies,
                model.Edition,
                model.AgeRestriction,
                model.ReleaseDate,
                model.AuthorId,
                model.Categories);

            if (bookId == 0)
            {
                return NotFound();
            }

            return Ok(bookId);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            int bookId = await this.bookService.DeleteAsync(id);

            if (bookId == 0)
            {
                return NotFound();
            }

            return Ok(bookId);
        }
    }
}