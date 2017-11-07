namespace Judge.App.Controllers
{
    using Infrastructure.Helpers;
    using Models.Contests;
    using Services.Contracts;
    using SimpleMvc.Framework.Attributes.Methods;
    using SimpleMvc.Framework.Contracts;
    using System.Collections.Generic;
    using System.Linq;
    using System;

    public class ContestsController : BaseController
    {
        private readonly IContestService contestService;

        public ContestsController(IContestService contestService)
        {
            this.contestService = contestService;
        }

        public IActionResult All()
        {
            if (!this.User.IsAuthenticated)
            {
                return this.RedirectToLogin();
            }

            IEnumerable<string> contests = null;

            if (this.IsAdmin)
            {
                contests = this.contestService.All().Select(c => c.ToAdminHtml());
            }
            else
            {
                contests = this.contestService.All().Select(c => c.ToHtml(this.Profile.Id));
            }

            this.ViewModel["contests"] = string.Join(string.Empty, contests);

            return this.View();
        }

        public IActionResult Create()
        {
            if (!this.User.IsAuthenticated)
            {
                return this.RedirectToLogin();
            }

            return this.View();
        }

        [HttpPost]
        public IActionResult Create(ContestModel model)
        {
            if (!this.User.IsAuthenticated)
            {
                return this.RedirectToLogin();
            }

            //if (!this.IsValidModel(model))
            //{
            //    this.ShowError(ErrorConstants.ContestNameError);

            //    return this.View();
            //}

            if (!this.ValidateModel(model))
            {
                return this.View();
            }

            this.contestService.Create(model.Name, this.Profile.Id);

            return this.RedirectToAll();
        }

        public IActionResult Edit(int id)
        {
            if (!this.User.IsAuthenticated)
            {
                return this.RedirectToLogin();
            }

            if (this.Profile.Id != this.contestService.GetContestAuthorId(id) && !this.IsAdmin)
            {
                return this.RedirectToLogin();
            }

            ContestModel model = this.contestService.GetById(id);

            if (model == null)
            {
                return this.NotFound();
            }

            this.SetViewModelData(model);

            return this.View();
        }

        [HttpPost]
        public IActionResult Edit(int id, ContestModel model)
        {
            if (!this.User.IsAuthenticated)
            {
                return this.RedirectToLogin();
            }

            if (!this.ValidateModel(model))
            {
                return this.View();
            }

            this.contestService.Edit(id, model.Name);

            return this.RedirectToAll();
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (!this.User.IsAuthenticated)
            {
                return this.RedirectToLogin();
            }

            if (this.Profile.Id != this.contestService.GetContestAuthorId(id) && !this.IsAdmin)
            {
                return this.RedirectToLogin();
            }

            ContestModel model = this.contestService.GetById(id);

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
            if (!this.User.IsAuthenticated)
            {
                return this.RedirectToLogin();
            }

            this.contestService.Delete(id);

            return this.RedirectToAll();
        }

        private bool ValidateModel(ContestModel model)
        {
            if (string.IsNullOrEmpty(model.Name) || model.Name.Length < 3 || model.Name.Length > 100)
            {
                this.ShowError(ErrorConstants.ContestLengthError);
                this.SetViewModelData(model);

                return false;
            }

            if (!char.IsUpper(model.Name.First()))
            {
                this.ShowError(ErrorConstants.ContestFirstLetterError);
                this.SetViewModelData(model);

                return false;
            }

            return true;
        }

        private void SetViewModelData(ContestModel model)
        {
            this.ViewModel["name"] = model.Name;
        }

        private IActionResult RedirectToAll()
        {
            return this.Redirect("/contests/all");
        }
    }
}