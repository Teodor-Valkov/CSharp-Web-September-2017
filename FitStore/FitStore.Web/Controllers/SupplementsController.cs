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

        public async Task<IActionResult> Details(int id, string name, string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;

            bool isSupplementExisting = await this.supplementService.IsSupplementExistingById(id, false);

            if (!isSupplementExisting)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, SupplementEntity));

                return RedirectToAction(nameof(HomeController.Index), Home);
            }

            SupplementDetailsServiceModel model = await this.supplementService.GetDetailsByIdAsync(id);

            return View(model);
        }
    }
}