namespace FluffyDuffyMunchkinCats.Handlers
{
    using Data;
    using Handlers.Contracts;
    using Infrastructure;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Linq;

    public class HomeHandler : IHandler
    {
        public int Order
        {
            get
            {
                return 1;
            }
        }

        public Func<HttpContext, bool> Condition
        {
            get
            {
                return context => context.Request.Path.Value == "/" && context.Request.Method == HttpMethod.Get;
            }
        }

        public RequestDelegate RequestHandler
        {
            get
            {
                return async (context) =>
                {
                    IHostingEnvironment environment = context.RequestServices.GetRequiredService<IHostingEnvironment>();

                    await context.Response.WriteAsync($"<h1>{environment.ApplicationName}</h1>");

                    CatsDbContext database = context.RequestServices.GetService<CatsDbContext>();

                    var catsData = database
                        .Cats
                        .Select(c => new
                        {
                            c.Id,
                            c.Name
                        })
                        .ToList();

                    await context.Response.WriteAsync("<ul>");

                    foreach (var cat in catsData)
                    {
                        await context.Response.WriteAsync($@"<li><a href=""/cats/{cat.Id}"">{cat.Name}</a></li>");
                    }

                    await context.Response.WriteAsync("</ul>");

                    await context.Response.WriteAsync(@"<form action=""/cats/add"">
                                                            <input type=""submit"" value=""Add Cat""/>
                                                        </form>");
                };
            }
        }
    }
}