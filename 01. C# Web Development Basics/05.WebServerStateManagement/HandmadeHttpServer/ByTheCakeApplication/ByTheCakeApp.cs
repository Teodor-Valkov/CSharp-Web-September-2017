namespace HandmadeHttpServer.ByTheCakeApplication
{
    using HandmadeHttpServer.ByTheCakeApplication.Controllers;
    using HandmadeHttpServer.ByTheCakeApplication.Data;
    using HandmadeHttpServer.ByTheCakeApplication.ViewModels.Account;
    using HandmadeHttpServer.ByTheCakeApplication.ViewModels.Product;
    using HandmadeHttpServer.Server.Contracts;
    using HandmadeHttpServer.Server.Routing.Contracts;
    using Microsoft.EntityFrameworkCore;

    public class ByTheCakeApp : IApplication
    {
        public void InitializeDatabase()
        {
            using (ByTheCakeDbContext database = new ByTheCakeDbContext())
            {
                database.Database.Migrate();
            }
        }

        public void Configure(IAppRouteConfig appRouteConfig)
        {
            appRouteConfig
                .Get(
                    "/",
                    httpRequest => new HomeController().Index());

            appRouteConfig
                .Get(
                    "/register",
                    httpRequest => new AccountController().Register());

            appRouteConfig
                .Post(
                    "/register",
                    httpRequest => new AccountController().Register(httpRequest, new RegisterViewModel
                    {
                        FullName = httpRequest.FormData["full-name"],
                        Username = httpRequest.FormData["username"],
                        Password = httpRequest.FormData["password"],
                        ConfirmPassword = httpRequest.FormData["confirm-password"]
                    }));

            appRouteConfig
               .Get(
                   "/login",
                   httpRequest => new AccountController().Login());

            appRouteConfig
                .Post(
                    "/login",
                    httpRequest => new AccountController().Login(httpRequest, new LoginViewModel
                    {
                        Username = httpRequest.FormData["username"],
                        Password = httpRequest.FormData["password"]
                    }));

            appRouteConfig
                .Get(
                    "/profile",
                    httpRequest => new AccountController().Profile(httpRequest));

            appRouteConfig
                .Post(
                    "/logout",
                    httpRequest => new AccountController().Logout(httpRequest));

            appRouteConfig
                .Get(
                    "/about",
                    httpRequest => new HomeController().About());

            appRouteConfig
                .Get(
                    "/add",
                    httpRequest => new ProductsController().Add());

            appRouteConfig
                .Post(
                    "/add",
                    httpRequest => new ProductsController().Add(
                        new AddProductViewModel
                        {
                            Name = httpRequest.FormData["name"],
                            Price = decimal.Parse(httpRequest.FormData["price"]),
                            ImageUrl = httpRequest.FormData["imageUrl"]
                        }));

            appRouteConfig
                .Get(
                    "/search",
                    httpRequest => new ProductsController().Search(httpRequest));

            appRouteConfig
                .Get(
                    "/products/{(?<id>[0-9]+)}",
                    req => new ProductsController().Details(int.Parse(req.UrlParameters["id"])));

            appRouteConfig
               .Get(
                   "/cart",
                   httpRequest => new ShoppingController().ShowCart(httpRequest));

            appRouteConfig
                .Get(
                    "/shopping/add/{(?<id>[0-9]+)}",
                    httpRequest => new ShoppingController().AddToCart(httpRequest));

            appRouteConfig
                .Post(
                    "/shopping/finish-order",
                    httpRequest => new ShoppingController().FinishOrder(httpRequest));

            appRouteConfig
                .Get(
                    "/orders",
                    httpRequest => new ShoppingController().ShowUserOrder(httpRequest));

            appRouteConfig
                .Get(
                    "/order-details/{(?<id>[0-9]+)}",
                    httpRequest => new ShoppingController().ShowOrder(httpRequest));
        }
    }
}