namespace FitStore.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using static Common.CommonConstants;

    public class BaseController : Controller
    {
        protected IActionResult RedirectToHomeIndex()
        {
            return RedirectToAction(nameof(HomeController.Index), Home);
        }

        protected string ReturnToHomeIndex()
        {
            return $"/";
        }

        protected string ReturnToCategoryDetails(int id, string name)
        {
            return $"/{Categories.ToLower()}/{nameof(CategoriesController.Details).ToLower()}/{id}?name={name}";
        }

        protected string ReturnToSubcategoryDetails(int id, string name)
        {
            return $"/{Subcategories.ToLower()}/{nameof(SubcategoriesController.Details).ToLower()}/{id}?name={name}";
        }

        protected string ReturnToManufacturerDetails(int id, string name)
        {
            return $"/{Manufacturers.ToLower()}/{nameof(ManufacturersController.Details).ToLower()}/{id}?name={name}";
        }
    }
}