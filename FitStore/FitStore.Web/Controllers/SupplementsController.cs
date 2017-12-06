namespace FitStore.Web.Controllers
{
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Services.Contracts;
    using Services.Models.Supplements;
    using System.Threading.Tasks;

    using static Common.CommonConstants;
    using static Common.CommonMessages;

    public class SupplementsController : Controller
    {
        private readonly ISupplementService supplementService;

        public SupplementsController(ISupplementService supplementService)
        {
            this.supplementService = supplementService;
        }

        public async Task<IActionResult> Details(int id, string name)
        {
            SupplementDetailsServiceModel model = await this.supplementService.GetDetailsByIdAsync(id);

            if (model == null)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, SupplementEntity, name));

                return RedirectToAction(nameof(SubcategoriesController.Index), Subcategories);
            }

            return View(model);
        }
    }
}