namespace GameStore.GameStoreApplication
{
    using GameStore.GameStoreApplication.Controllers;
    using GameStore.GameStoreApplication.Data;
    using GameStore.GameStoreApplication.ViewModels.Account;
    using GameStore.GameStoreApplication.ViewModels.Admin;
    using GameStore.Server.Contracts;
    using GameStore.Server.Routing.Contracts;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Globalization;

    public class GameStoreApp : IApplication
    {
        public void InitializeDatabase()
        {
            using (GameStoreDbContext database = new GameStoreDbContext())
            {
                database.Database.Migrate();
            }
        }

        public void Configure(IAppRouteConfig appRouteConfig)
        {
            appRouteConfig.AnonymousPaths.Add("^/$");
            appRouteConfig.AnonymousPaths.Add("^/account/login$");
            appRouteConfig.AnonymousPaths.Add("^/account/register$");
            appRouteConfig.AnonymousPaths.Add("^/account/games");
            appRouteConfig.AnonymousPaths.Add("^/games/details/[0-9]+$");
            appRouteConfig.AnonymousPaths.Add("^/shopping/cart/details$");
            appRouteConfig.AnonymousPaths.Add("^/shopping/cart/add/[0-9]+$");
            appRouteConfig.AnonymousPaths.Add("^/shopping/cart/remove/[0-9]+$");
            appRouteConfig.AnonymousPaths.Add("^/shopping/cart/order$");

            appRouteConfig
                .Get(
                    "/",
                    httpRequest => new HomeController(httpRequest).Index());

            appRouteConfig
                .Get(
                    "/account/register",
                    httpRequest => new AccountController(httpRequest).Register());

            appRouteConfig
                .Post(
                    "/account/register",
                    httpRequest => new AccountController(httpRequest).Register(new RegisterViewModel
                    {
                        Email = httpRequest.FormData["email"],
                        FullName = httpRequest.FormData["full-name"],
                        Password = httpRequest.FormData["password"],
                        ConfirmPassword = httpRequest.FormData["confirm-password"]
                    }));

            appRouteConfig
                .Get(
                    "/account/login",
                    httpRequest => new AccountController(httpRequest).Login());

            appRouteConfig
                .Post(
                    "/account/login",
                    httpRequest => new AccountController(httpRequest).Login(new LoginViewModel
                    {
                        Email = httpRequest.FormData["email"],
                        Password = httpRequest.FormData["password"]
                    }));

            appRouteConfig
                .Get(
                    "/account/logout",
                    httpRequest => new AccountController(httpRequest).Logout());

            appRouteConfig
                .Get(
                    "/account/games",
                    httpRequest => new HomeController(httpRequest).IndexFiltered());

            appRouteConfig
                .Get(
                    "/admin/games/list",
                    httpRequest => new AdminController(httpRequest).ListGames());

            appRouteConfig
                .Get(
                    "/admin/games/add",
                    httpRequest => new AdminController(httpRequest).AddGame());

            appRouteConfig
                .Post(
                    "/admin/games/add",
                    httpRequest => new AdminController(httpRequest).AddGame(new AdminAddGameViewModel
                    {
                        Title = httpRequest.FormData["title"],
                        Description = httpRequest.FormData["description"],
                        ImageUrl = httpRequest.FormData["image-url"],
                        Price = decimal.Parse(httpRequest.FormData["price"]),
                        Size = double.Parse(httpRequest.FormData["size"]),
                        VideoId = httpRequest.FormData["video-id"],
                        ReleaseDate = DateTime.ParseExact(httpRequest.FormData["release-date"], "yyyy-MM-dd", CultureInfo.InvariantCulture)
                    }));

            appRouteConfig
                .Get(
                    "/admin/games/edit/{(?<id>[0-9]+)}",
                    httpRequest => new AdminController(httpRequest).EditGame());

            appRouteConfig
                .Post(
                    "/admin/games/edit/{(?<id>[0-9]+)}",
                    httpRequest => new AdminController(httpRequest).EditGame(new AdminDetailsGameViewModel
                    {
                        Title = httpRequest.FormData["title"],
                        Description = httpRequest.FormData["description"],
                        ImageUrl = httpRequest.FormData["image-url"],
                        Size = double.Parse(httpRequest.FormData["size"]),
                        Price = decimal.Parse(httpRequest.FormData["price"]),
                        VideoId = httpRequest.FormData["video-id"],
                        ReleaseDate = DateTime.ParseExact(httpRequest.FormData["release-date"], "yyyy-MM-dd", CultureInfo.InvariantCulture)
                    }));

            appRouteConfig
              .Get(
                  "/admin/games/delete/{(?<id>[0-9]+)}",
                  httpRequest => new AdminController(httpRequest).DeleteGame());

            appRouteConfig
                .Post(
                  "/admin/games/delete/{(?<id>[0-9]+)}",
                    httpRequest => new AdminController(httpRequest).ConfirmDeleteGame());

            appRouteConfig
               .Get(
                   "/games/details/{(?<id>[0-9]+)}",
                   httpRequest => new GameController(httpRequest).GameDetails());

            appRouteConfig
             .Get(
                 "/shopping/cart/details",
                 httpRequest => new ShoppingController(httpRequest).ShowCart());

            appRouteConfig
             .Get(
                 "/shopping/cart/add/{(?<id>[0-9]+)}",
                 httpRequest => new ShoppingController(httpRequest).AddToCart());

            appRouteConfig
             .Get(
                 "/shopping/cart/remove/{(?<id>[0-9]+)}",
                 httpRequest => new ShoppingController(httpRequest).RemoveFromCart());

            appRouteConfig
             .Post(
               "/shopping/cart/order",
               httpRequest => new ShoppingController(httpRequest).CreateOrder());
        }
    }
}