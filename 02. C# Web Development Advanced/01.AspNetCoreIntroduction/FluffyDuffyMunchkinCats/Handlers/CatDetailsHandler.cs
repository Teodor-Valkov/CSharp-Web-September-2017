namespace FluffyDuffyMunchkinCats.Handlers
{
    using Data;
    using Data.Models;
    using Handlers.Contracts;
    using Infrastructure;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using System;

    public class CatDetailsHandler : IHandler
    {
        public int Order
        {
            get
            {
                return 3;
            }
        }

        public Func<HttpContext, bool> Condition
        {
            get
            {
                return context => context.Request.Path.Value.StartsWith("/cats") && context.Request.Method == HttpMethod.Get;
            }
        }

        public RequestDelegate RequestHandler
        {
            get
            {
                return async (context) =>
                {
                    string[] urlParts = context.Request.Path.Value.Split('/', StringSplitOptions.RemoveEmptyEntries);

                    if (urlParts.Length != 2)
                    {
                        context.Response.Redirect("/");
                        return;
                    }

                    int catId = 0;
                    int.TryParse(urlParts[1], out catId);

                    if (catId == 0)
                    {
                        context.Response.Redirect("/");
                        return;
                    }

                    CatsDbContext database = context.RequestServices.GetRequiredService<CatsDbContext>();

                    using (database)
                    {
                        Cat cat = await database.Cats.FindAsync(catId);

                        if (cat == null)
                        {
                            context.Response.Redirect("/");
                            return;
                        }

                        await context.Response.WriteAsync(@"<a href=""/"">Back To Home</a>");
                        await context.Response.WriteAsync($"<h1>{cat.Name}</h1>");
                        await context.Response.WriteAsync($@"<img src=""{cat.ImageUrl}"" alt=""{cat.Name}"" width=""300""/>");
                        await context.Response.WriteAsync($"<p>Age: {cat.Age}</p>");
                        await context.Response.WriteAsync($"<p>Breed: {cat.Breed}</p>");
                    }
                };
            }
        }
    }
}