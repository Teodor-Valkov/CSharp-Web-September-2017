namespace FitStore.Web.Areas.Manager.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using static Common.CommonConstants;
    using static WebConstants;

    [Area(ManagerArea)]
    [Authorize(Roles = ManagerRole)]
    public class BaseManagerController : Controller
    {
        protected RedirectToActionResult RedirectToCategoriesIndex(bool isDeleted)
        {
            return RedirectToAction(nameof(CategoriesController.Index), Categories, new { isDeleted });
        }

        protected RedirectToActionResult RedirectToSubcategoriesIndex(bool isDeleted)
        {
            return RedirectToAction(nameof(SubcategoriesController.Index), Subcategories, new { isDeleted });
        }

        protected RedirectToActionResult RedirectToManufacturersIndex(bool isDeleted)
        {
            return RedirectToAction(nameof(ManufacturersController.Index), Manufacturers, new { isDeleted });
        }

        protected RedirectToActionResult RedirectToSupplementsIndex(bool isDeleted)
        {
            return RedirectToAction(nameof(SupplementsController.Index), Supplements, new { isDeleted });
        }

        protected string ReturnToSupplementsIndex()
        {
            return $"/{ManagerRole.ToLower()}/{Supplements.ToLower()}";
        }
    }
}