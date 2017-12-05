namespace BookShop.Api.Controllers
{
    using Infrastructure.Filters;
    using Microsoft.AspNetCore.Mvc;
    using Models.Categories;
    using Services.Contracts;
    using Services.Models.Categories;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class CategoriesController : BaseController
    {
        private readonly ICategoryService categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            IEnumerable<CategoryDetailsServiceModel> model = await this.categoryService.GetAllListingAsync();

            if (model == null)
            {
                return NotFound();
            }

            return Ok(model);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            CategoryDetailsServiceModel model = await this.categoryService.GetCategoryDetailsByIdAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            return Ok(model);
        }

        [HttpPost]
        [ValidateModelState]
        public async Task<IActionResult> Post([FromBody] CategoryRequestPostModel model)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            int categoryId = await this.categoryService.CreateAsync(model.Name);

            if (categoryId == 0)
            {
                return BadRequest("There is already a category with that name.");
            }

            return Ok(categoryId);
        }

        [HttpPut("{id}")]
        [ValidateModelState]
        public async Task<IActionResult> Put(int id, [FromBody]CategoryRequestPostModel model)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            bool isCategoryExisting = await this.categoryService.IsCategoryExisting(id);

            if (!isCategoryExisting)
            {
                return NotFound();
            }

            int categoryId = await this.categoryService.EditAsync(id, model.Name);

            if (categoryId == 0)
            {
                return BadRequest("There is already a category with that name.");
            }

            return Ok(categoryId);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            bool isCategoryExisting = await this.categoryService.IsCategoryExisting(id);

            if (!isCategoryExisting)
            {
                return NotFound();
            }

            int categoryId = await this.categoryService.DeleteAsync(id);

            return Ok(categoryId);
        }
    }
}