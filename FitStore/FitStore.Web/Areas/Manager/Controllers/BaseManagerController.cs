namespace FitStore.Web.Areas.Manager.Controllers
{
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using static Common.CommonConstants;
    using static WebConstants;

    [Area(ManagerArea)]
    [Authorize(Roles = AdministratorRole)]
    public class BaseManagerController : Controller
    {
        protected void AddSuccessMessage(string message)
        {
            TempData.AddSuccessMessage(message);
        }

        protected void AddErrorMessage(string message)
        {
            TempData.AddErrorMessage(message);
        }

        protected RedirectToActionResult RedirectToCategoriesIndex()
        {
            return RedirectToAction(nameof(Web.Controllers.CategoriesController.Index), Categories, new { area = "" });
        }

        protected RedirectToActionResult RedirectToSubcategoriesIndex()
        {
            return RedirectToAction(nameof(Web.Controllers.SubcategoriesController.Index), Subcategories, new { area = "" });
        }

        protected RedirectToActionResult RedirectToManufacturersIndex()
        {
            return RedirectToAction(nameof(Web.Controllers.ManufacturersController.Index), Manufacturers, new { area = "" });
        }
    }
}