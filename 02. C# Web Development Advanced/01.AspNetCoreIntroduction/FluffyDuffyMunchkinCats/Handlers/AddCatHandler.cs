namespace FluffyDuffyMunchkinCats.Handlers
{
    using Data;
    using Data.Models;
    using Handlers.Contracts;
    using Infrastructure;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using System;

    public class AddCatHandler : IHandler
    {
        public int Order
        {
            get
            {
                return 2;
            }
        }

        public Func<HttpContext, bool> Condition
        {
            get
            {
                return context => context.Request.Path.Value == "/cats/add";
            }
        }

        public RequestDelegate RequestHandler
        {
            get
            {
                return async (context) =>
                {
                    if (context.Request.Method == HttpMethod.Get)
                    {
                        context.Response.Redirect("/cats-add.html");
                    }
                    else if (context.Request.Method == HttpMethod.Post)
                    {
                        IFormCollection formData = context.Request.Form;

                        int age = 0;
                        int.TryParse(formData["Age"], out age);

                        Cat cat = new Cat
                        {
                            Name = formData["Name"],
                            Age = age,
                            Breed = formData["Breed"],
                            ImageUrl = formData["ImageUrl"]
                        };

                        try
                        {
                            if (string.IsNullOrWhiteSpace(cat.Name) || string.IsNullOrWhiteSpace(cat.Breed) || string.IsNullOrWhiteSpace(cat.ImageUrl))
                            {
                                throw new InvalidOperationException("Invalid cat data!");
                            }

                            CatsDbContext database = context.RequestServices.GetRequiredService<CatsDbContext>();

                            using (database)
                            {
                                database.Cats.Add(cat);

                                await database.SaveChangesAsync();
                            }

                            context.Response.Redirect("/");
                        }
                        catch
                        {
                            await context.Response.WriteAsync("<h2>Invalid cat data!</h2>");
                            await context.Response.WriteAsync(@"<a href=""/cats/add"">Back To Form</a>");
                        }
                    }
                };
            }
        }
    }
}