namespace AdvancedMvc.App.Controllers
{
    using BindingModels;
    using Data;
    using Framework.Attributes.Methods;
    using Framework.Contracts;
    using Framework.Controllers;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using System.Collections.Generic;
    using System.Linq;

    public class UsersController : Controller
    {
        [HttpGet]
        public IActionResult Register()
        {
            return this.View();
        }

        [HttpPost]
        public IActionResult Register(RegisterUserBindingModel model)
        {
            if (!this.IsModelValid(model))
            {
                return this.View();
            }

            User user = new User
            {
                Username = model.Username,
                Password = model.Password
            };

            using (AdvancedMvcDbContext database = new AdvancedMvcDbContext())
            {
                database.Users.Add(user);
                database.SaveChanges();
            }

            return this.View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return this.View();
        }

        [HttpPost]
        public IActionResult Login(LoginUserBindingModel model)
        {
            if (!this.IsModelValid(model))
            {
                return this.View();
            }

            using (AdvancedMvcDbContext database = new AdvancedMvcDbContext())
            {
                User user = database.Users.FirstOrDefault(u => u.Username == model.Username && u.Password == model.Password);

                if (user == null)
                {
                    //return this.Login();
                    return this.RedirectToAction("/home/login");
                }

                this.SignIn(user.Username);
            }

            return this.RedirectToAction("home/index");
        }

        [HttpGet]
        public IActionResult All()
        {
            if (!this.User.IsAuthenticated)
            {
                return this.RedirectToAction("/users/login");
            }

            IDictionary<int, string> users = new Dictionary<int, string>();

            using (AdvancedMvcDbContext database = new AdvancedMvcDbContext())
            {
                users = database.Users.ToDictionary(u => u.Id, u => u.Username);
            }

            this.Model["users"] = users.Any()
                ? string.Join(string.Empty, users.Select(u => $@"<li><a href=""/users/profile?id={u.Key}"">{u.Value}</a></li>"))
                : string.Empty;

            return this.View();
        }

        [HttpGet]
        public IActionResult Profile(int id)
        {
            using (AdvancedMvcDbContext database = new AdvancedMvcDbContext())
            {
                User user = database.Users.Include(u => u.Notes).FirstOrDefault(u => u.Id == id);

                this.Model["userId"] = user.Id.ToString();
                this.Model["username"] = user.Username;
                this.Model["notes"] = user.Notes.Any()
                    ? string.Join(string.Empty, user.Notes.Select(n => $@"<li>{n.Title} - {n.Content}</li>"))
                    : string.Empty;

                return this.View();
            }
        }

        [HttpPost]
        public IActionResult Profile(AddNoteBindingModel model)
        {
            if (!this.IsModelValid(model))
            {
                return this.View();
            }

            using (AdvancedMvcDbContext database = new AdvancedMvcDbContext())
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

        [HttpPost]
        public IActionResult Logout()
        {
            this.SignOut();

            return this.RedirectToAction("/home/index");
        }
    }
}