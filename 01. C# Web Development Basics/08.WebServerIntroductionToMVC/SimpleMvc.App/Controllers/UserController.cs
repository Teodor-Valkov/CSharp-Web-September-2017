namespace SimpleMvc.App.Controllers
{
    using BindingModels;
    using Data;
    using Framework.Attributes.Methods;
    using Framework.Contracts;
    using Framework.Contracts.Generic;
    using Framework.Controllers;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using System.Collections.Generic;
    using System.Linq;
    using ViewModels;

    public class UserController : Controller
    {
        [HttpGet]
        public IActionResult Register()
        {
            return this.View();
        }

        [HttpPost]
        public IActionResult Register(RegisterUserBindingModel model)
        {
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                return this.Register();
            }

            User user = new User
            {
                Username = model.Username,
                Password = model.Password
            };

            using (SimpleMvcDbContext database = new SimpleMvcDbContext())
            {
                database.Users.Add(user);
                database.SaveChanges();
            }

            return this.View();
        }

        [HttpGet]
        public IActionResult<AllUsersViewModel> All()
        {
            IList<UserViewModel> users = null;

            using (SimpleMvcDbContext database = new SimpleMvcDbContext())
            {
                users = database.Users
                    .Select(u => new UserViewModel
                    {
                        UserId = u.Id,
                        Username = u.Username
                    }).ToList();
            }

            AllUsersViewModel model = new AllUsersViewModel
            {
                Users = users
            };

            return this.View(model);
        }

        [HttpGet]
        public IActionResult<UserProfileViewModel> Profile(int id)
        {
            using (SimpleMvcDbContext database = new SimpleMvcDbContext())
            {
                User user = database.Users.Include(u => u.Notes).FirstOrDefault(u => u.Id == id);

                UserProfileViewModel model = new UserProfileViewModel
                {
                    UserId = user.Id,
                    Username = user.Username,
                    Notes = user.Notes
                        .Select(n => new NoteViewModel
                        {
                            Title = n.Title,
                            Content = n.Content
                        })
                };

                return this.View(model);
            }
        }

        [HttpPost]
        public IActionResult<UserProfileViewModel> Profile(AddNoteBindingModel model)
        {
            using (SimpleMvcDbContext database = new SimpleMvcDbContext())
            {
                User user = database.Users.FirstOrDefault(u => u.Id == model.UserId);

                Note note = new Note
                {
                    Title = model.Title,
                    Content = model.Content
                };

                user.Notes.Add(note);
                database.SaveChanges();
            }

            return this.Profile(model.UserId);
        }
    }
}