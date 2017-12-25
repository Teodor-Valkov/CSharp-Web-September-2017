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
            if (!Url.IsLocalUrl(returnUrl))
            {
                returnUrl = this.ReturnToHomeIndex();
            }

            bool isSupplementExisting = await this.supplementService.IsSupplementExistingById(id, false);

            if (!isSupplementExisting)
            {
                TempData.AddErrorMessage(string.Format(EntityNotFound, SupplementEntity));

                return this.RedirectToHomeIndex();
            }

            ShoppingCart shoppingCart = HttpContext.Session.GetShoppingCart<ShoppingCart>(UserSessionShoppingCartKey);

            bool alreadyAddedLastAvailableSupplement = await this.orderService.IsLastAvailableSupplementAlreadyAdded(id, shoppingCart);

            if (alreadyAddedLastAvailableSupplement)
            {
                TempData.AddErrorMessage(SupplementLastOneJustAddedErrorMessage);

                return this.Redirect(returnUrl);
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

            return this.Redirect(returnUrl);
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
                TempData.AddErrorMessage(SupplementCannotBeRemovedFromCartErrorMessage);
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

            bool removeAllSupplementsFromCartResult = this.orderService.RemoveAllSupplementsFromCartAsync(id, shoppingCart);

            if (removeAllSupplementsFromCartResult)
            {
                TempData.AddSuccessMessage(SupplementRemovedFromCartSuccessMessage);

                HttpContext.Session.SetShoppingCart(UserSessionShoppingCartKey, shoppingCart);
            }
            else
            {
                TempData.AddErrorMessage(SupplementCannotBeRemovedFromCartErrorMessage);
            }

            return RedirectToAction(nameof(Details));
        }

        public IActionResult Details()
        {
            ShoppingCart shoppingCart = HttpContext.Session.GetShoppingCart<ShoppingCart>(UserSessionShoppingCartKey);

            return View(shoppingCart);
        }

        [Authorize]
        public IActionResult Checkout()
        {
            ShoppingCart shoppingCart = HttpContext.Session.GetShoppingCart<ShoppingCart>(UserSessionShoppingCartKey);

            if (!shoppingCart.Supplements.Any())
            {
                return this.RedirectToHomeIndex();
            }

            return View(shoppingCart);
        }

        [Authorize]
        public IActionResult Cancel()
        {
            ShoppingCart shoppingCart = HttpContext.Session.GetShoppingCart<ShoppingCart>(UserSessionShoppingCartKey);

            if (!shoppingCart.Supplements.Any())
            {
                return this.RedirectToHomeIndex();
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
                return this.RedirectToHomeIndex();
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

            return this.RedirectToHomeIndex();
        }

        [Authorize]
        public async Task<IActionResult> Review(int id)
        {
            bool isOrderExistingById = await this.orderService.IsOrderExistingById(id);

            if (!isOrderExistingById)
            {
                return this.RedirectToHomeIndex();
            }

            OrderDetailsServiceModel model = await this.orderService.GetDetailsByIdAsync(id);

            return View(model);
        }
    }
}