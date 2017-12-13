namespace FitStore.Web.Controllers
{
    using Data.Models;
    using FitStore.Services.Models.Orders;
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Services.Contracts;
    using Services.Models;
    using System.Linq;
    using System.Threading.Tasks;

    using static Common.CommonConstants;
    using static Common.CommonMessages;

    public class OrdersController : BaseController
    {
        private readonly UserManager<User> userManager;
        private readonly IOrderService orderService;
        private readonly ISupplementService supplementService;

        public OrdersController(UserManager<User> userManager, IOrderService orderService, ISupplementService supplementService)
        {
            this.userManager = userManager;
            this.orderService = orderService;
            this.supplementService = supplementService;
        }

        public async Task<IActionResult> Add(int id, string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;

            bool isSupplementExisting = await this.supplementService.IsSupplementExistingById(id, false);

            if (!isSupplementExisting)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, SupplementEntity));

                return RedirectToAction(nameof(HomeController.Index), Home);
            }

            ShoppingCart shoppingCart = HttpContext.Session.GetShoppingCart(UserSessionShoppingCartKeyName);

            bool addSupplementToCartResult = await this.orderService.AddSupplementToCartAsync(id, shoppingCart);

            if (addSupplementToCartResult && returnUrl == null)
            {
                TempData.AddSuccessMessage(SupplementAddedToCartSuccessMessage);
            }
            else if (!addSupplementToCartResult)
            {
                TempData.AddErrorMessage(SupplementUnavailableErrorMessage);
            }

            HttpContext.Session.SetShoppingCart(UserSessionShoppingCartKeyName, shoppingCart);

            return this.RedirectToLocal(returnUrl);
        }

        public async Task<IActionResult> Remove(int id)
        {
            bool isSupplementExisting = await this.supplementService.IsSupplementExistingById(id, false);

            if (!isSupplementExisting)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, SupplementEntity));

                return RedirectToAction(nameof(Details));
            }

            ShoppingCart shoppingCart = HttpContext.Session.GetShoppingCart(UserSessionShoppingCartKeyName);

            bool removeSupplementFromCartResult = await this.orderService.RemoveSupplementFromCartAsync(id, shoppingCart);

            if (removeSupplementFromCartResult)
            {
                HttpContext.Session.SetShoppingCart(UserSessionShoppingCartKeyName, shoppingCart);
            }
            else
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, SupplementEntity));
            }

            return RedirectToAction(nameof(Details));
        }

        public async Task<IActionResult> RemoveAll(int id)
        {
            bool isSupplementExisting = await this.supplementService.IsSupplementExistingById(id, false);

            if (!isSupplementExisting)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, SupplementEntity));

                return RedirectToAction(nameof(Details));
            }

            ShoppingCart shoppingCart = HttpContext.Session.GetShoppingCart(UserSessionShoppingCartKeyName);

            bool removeSupplementFromCartResult = await this.orderService.RemoveAllSupplementsFromCartAsync(id, shoppingCart);

            if (removeSupplementFromCartResult)
            {
                TempData.AddSuccessMessage(SupplementRemovedFromCartSuccessMessage);

                HttpContext.Session.SetShoppingCart(UserSessionShoppingCartKeyName, shoppingCart);
            }
            else
            {
                TempData.AddErrorMessage(SupplementRemovedFromCartErrorMessage);
            }

            return RedirectToAction(nameof(Details));
        }

        public IActionResult Details()
        {
            ShoppingCart shoppingCart = HttpContext.Session.GetShoppingCart(UserSessionShoppingCartKeyName);

            return this.View(shoppingCart);
        }

        [Authorize]
        public IActionResult Checkout()
        {
            ShoppingCart shoppingCart = HttpContext.Session.GetShoppingCart(UserSessionShoppingCartKeyName);

            if (!shoppingCart.Supplements.Any())
            {
                return RedirectToAction(nameof(HomeController.Index), Home);
            }

            return View(shoppingCart);
        }

        [Authorize]
        public async Task<IActionResult> Cancel()
        {
            ShoppingCart shoppingCart = HttpContext.Session.GetShoppingCart(UserSessionShoppingCartKeyName);

            if (!shoppingCart.Supplements.Any())
            {
                return RedirectToAction(nameof(HomeController.Index), Home);
            }

            await this.orderService.CancelOrderAsync(shoppingCart);

            HttpContext.Session.Clear();

            TempData.AddSuccessMessage(CancelOrderSuccessMessage);

            return RedirectToAction(nameof(Details));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Order()
        {
            string userId = this.userManager.GetUserId(User);

            ShoppingCart shoppingCart = HttpContext.Session.GetShoppingCart(UserSessionShoppingCartKeyName);

            if (!shoppingCart.Supplements.Any())
            {
                return RedirectToAction(nameof(HomeController.Index), Home);
            }

            await this.orderService.FinishOrderAsync(userId, shoppingCart);

            HttpContext.Session.Clear();

            TempData.AddSuccessMessage(FinishOrderSuccessMessage);

            return RedirectToAction(nameof(HomeController.Index), Home);
        }

        [Authorize]
        public async Task<IActionResult> Review(int id)
        {
            bool isOrderExistingById = await this.orderService.IsOrderExistingById(id);

            if (!isOrderExistingById)
            {
                return RedirectToAction(nameof(HomeController.Index), Home);
            }

            OrderDetailsServiceModel model = await this.orderService.GetDetailsByIdAsync(id);

            ViewData["ReturnUrl"] = this.RedirectToOrderReview(id);

            return View(model);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
    }
}