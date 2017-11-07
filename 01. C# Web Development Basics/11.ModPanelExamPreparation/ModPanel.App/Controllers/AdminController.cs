namespace ModPanel.App.Controllers
{
    using Data.Models.Enums;
    using Infrastructure.Helpers;
    using Models.Posts;
    using Services.Contracts;
    using SimpleMvc.Framework.Attributes.Methods;
    using SimpleMvc.Framework.Contracts;
    using System.Collections.Generic;
    using System.Linq;

    public class AdminController : BaseController
    {
        private readonly IUserService userService;
        private readonly IPostService postService;
        private readonly ILogService logService;

        public AdminController(IUserService userService, IPostService postService, ILogService logService)
        {
            this.userService = userService;
            this.postService = postService;
            this.logService = logService;
        }

        [HttpGet]
        public IActionResult Users()
        {
            if (!this.IsAdmin)
            {
                return this.RedirectToLogin();
            }

            IEnumerable<string> users = this.userService
                .All()
                .Select(u => u.ToHtml());

            this.ViewModel["users"] = string.Join(string.Empty, users);

            this.Log(LogType.OpenMenu, nameof(Users));

            return this.View();
        }

        [HttpGet]
        public IActionResult Approve(int id)
        {
            if (!this.IsAdmin)
            {
                return this.RedirectToLogin();
            }

            string userEmail = this.userService.Approve(id);

            if (userEmail != null)
            {
                this.Log(LogType.UserApproval, userEmail);
            }

            return this.Redirect(HtmlHelpers.ToAdminUsers());
        }

        [HttpGet]
        public IActionResult Posts()
        {
            if (!this.IsAdmin)
            {
                return this.RedirectToLogin();
            }

            IEnumerable<string> posts = this.postService
                .All()
                .Select(p => p.ToHtml());

            this.ViewModel["posts"] = string.Join(string.Empty, posts);

            this.Log(LogType.OpenMenu, nameof(Posts));

            return this.View();
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (!this.IsAdmin)
            {
                return this.RedirectToLogin();
            }

            PostModel model = this.postService.GetById(id);

            if (model == null)
            {
                return this.NotFound();
            }

            this.SetViewModelData(model);

            return this.View();
        }

        [HttpPost]
        public IActionResult Edit(int id, PostModel model)
        {
            if (!this.IsAdmin)
            {
                return this.RedirectToLogin();
            }

            if (!this.IsValidModel(model))
            {
                this.ShowError(ErrorConstants.EditError);
                SetViewModelData(model);

                return this.View();
            }

            this.postService.Edit(id, model.Title, model.Content);

            this.Log(LogType.EditPost, model.Title);

            return this.Redirect(HtmlHelpers.ToAdminPosts());
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (!this.IsAdmin)
            {
                return this.RedirectToLogin();
            }

            PostModel model = this.postService.GetById(id);

            if (model == null)
            {
                return this.NotFound();
            }

            this.ViewModel["id"] = id.ToString();
            this.SetViewModelData(model);

            return this.View();
        }

        [HttpPost]
        public IActionResult Confirm(int id)
        {
            if (!this.IsAdmin)
            {
                return this.RedirectToLogin();
            }

            string postTitle = this.postService.Delete(id);

            if (postTitle != null)
            {
                this.Log(LogType.DeletePost, postTitle);
            }

            return this.Redirect(HtmlHelpers.ToAdminPosts());
        }

        public IActionResult Log()
        {
            this.Log(LogType.OpenMenu, nameof(Log));

            IEnumerable<string> logs = this.logService
                .All()
                .Select(l => l.ToHtml());

            this.ViewModel["logs"] = string.Join(string.Empty, logs);

            return this.View();
        }

        private void SetViewModelData(PostModel model)
        {
            this.ViewModel["title"] = model.Title;
            this.ViewModel["content"] = model.Content;
        }
    }
}