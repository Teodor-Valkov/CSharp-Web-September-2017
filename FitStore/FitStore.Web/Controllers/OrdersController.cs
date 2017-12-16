namespace FitStore.Web.Controllers
{
    using Data.Models;
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Services.Contracts;
    using Services.Models;
    using Services.Models.Orders;
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

            ShoppingCart shoppingCart = HttpContext.Session.GetShoppingCart<ShoppingCart>(UserSessionShoppingCartKey);

            bool alreadyAddedLastAvailableSupplement = await this.orderService.IsLastAvailableSupplementAlreadyAdded(id, shoppingCart);

            if (alreadyAddedLastAvailableSupplement)
            {
                TempData.AddErrorMessage(SupplementLastOneJustAddedErrorMessage);

                return this.RedirectToLocal(returnUrl);
            }

            bool addSupplementToCartResult = await this.orderService.AddSupplementToCartAsync(id, shoppingCart);

            if (addSupplementToCartResult)
            {
                TempData.AddSuccessMessage(SupplementAddedToCartSuccessMessage);

                HttpContext.Session.SetShoppingCart(UserSessionShoppingCartKey, shoppingCart);
            }
            else
            {
                TempData.AddErrorMessage(SupplementUnavailableErrorMessage);
            }

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

            ShoppingCart shoppingCart = HttpContext.Session.GetShoppingCart<ShoppingCart>(UserSessionShoppingCartKey);

            bool removeSupplementFromCartResult = this.orderService.RemoveSupplementFromCartAsync(id, shoppingCart);

            if (removeSupplementFromCartResult)
            {
                TempData.AddSuccessMessage(SupplementRemovedFromCartSuccessMessage);

                HttpContext.Session.SetShoppingCart(UserSessionShoppingCartKey, shoppingCart);
            }
            else
            {
                TempData.AddErrorMessage(SupplementRemovedFromCartErrorMessage);
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

            ShoppingCart shoppingCart = HttpContext.Session.GetShoppingCart<ShoppingCart>(UserSessionShoppingCartKey);

            bool removeSupplementFromCartResult = this.orderService.RemoveAllSupplementsFromCartAsync(id, shoppingCart);

            if (removeSupplementFromCartResult)
            {
                TempData.AddSuccessMessage(SupplementRemovedFromCartSuccessMessage);

                HttpContext.Session.SetShoppingCart(UserSessionShoppingCartKey, shoppingCart);
            }
            else
            {
                TempData.AddErrorMessage(SupplementRemovedFromCartErrorMessage);
            }

            return RedirectToAction(nameof(Details));
        }

        public IActionResult Details()
        {
            ShoppingCart shoppingCart = HttpContext.Session.GetShoppingCart<ShoppingCart>(UserSessionShoppingCartKey);

            return this.View(shoppingCart);
        }

        [Authorize]
        public IActionResult Checkout()
        {
            ShoppingCart shoppingCart = HttpContext.Session.GetShoppingCart<ShoppingCart>(UserSessionShoppingCartKey);

            if (!shoppingCart.Supplements.Any())
            {
                return RedirectToAction(nameof(HomeController.Index), Home);
            }

            return View(shoppingCart);
        }

        [Authorize]
        public IActionResult Cancel()
        {
            ShoppingCart shoppingCart = HttpContext.Session.GetShoppingCart<ShoppingCart>(UserSessionShoppingCartKey);

            if (!shoppingCart.Supplements.Any())
            {
                return RedirectToAction(nameof(HomeController.Index), Home);
            }

            HttpContext.Session.Remove(UserSessionShoppingCartKey);

            TempData.AddSuccessMessage(CancelOrderSuccessMessage);

            return RedirectToAction(nameof(Details));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Order()
        {
            string userId = this.userManager.GetUserId(User);

            ShoppingCart shoppingCart = HttpContext.Session.GetShoppingCart<ShoppingCart>(UserSessionShoppingCartKey);

            if (!shoppingCart.Supplements.Any())
            {
                return RedirectToAction(nameof(HomeController.Index), Home);
            }

            bool OrderResult = await this.orderService.FinishOrderAsync(userId, shoppingCart);

            if (OrderResult)
            {
                TempData.AddSuccessMessage(FinishOrderSuccessMessage);
            }
            else
            {
                TempData.AddErrorMessage(FinishOrderErrorMessage);
            }

            HttpContext.Session.Remove(UserSessionShoppingCartKey);

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
    }
}