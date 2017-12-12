namespace FitStore.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using static Common.CommonConstants;

    public class BaseController : Controller
    {
        //protected IActionResult RedirectToLocal(string returnUrl)
        //{
        //    if (Url.IsLocalUrl(returnUrl))
        //    {
        //        return Redirect(returnUrl);
        //    }
        //    else
        //    {
        //        return RedirectToAction(nameof(HomeController.Index), Home);
        //    }
        //}

        protected string RedirectToHomeIndex()
        {
            return $"/{Home.ToLower()}/{nameof(HomeController.Index).ToLower()}";
        }

        protected string RedirectToCategoryDetails(int id, string name)
        {
            return $"/{Categories.ToLower()}/{nameof(CategoriesController.Details).ToLower()}/{id}?name={name}";
        }

        protected string RedirectToSubcategoryDetails(int id, string name)
        {
            return $"/{Subcategories.ToLower()}/{nameof(SubcategoriesController.Details).ToLower()}/{id}?name={name}";
        }

        protected string RedirectToManufacturerDetails(int id, string name)
        {
            return $"/{Manufacturers.ToLower()}/{nameof(ManufacturersController.Details).ToLower()}/{id}?name={name}";
        }
    }
}