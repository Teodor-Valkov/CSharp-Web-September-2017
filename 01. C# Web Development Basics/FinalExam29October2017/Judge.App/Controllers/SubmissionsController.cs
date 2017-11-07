namespace Judge.App.Controllers
{
    using Data.Models.Enums;
    using Infrastructure.Helpers;
    using Models.Contests;
    using Models.Submissions;
    using Services.Contracts;
    using SimpleMvc.Framework.Attributes.Methods;
    using SimpleMvc.Framework.Contracts;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class SubmissionsController : BaseController
    {
        private readonly ISubmissionService submissionService;
        private readonly IContestService contestService;

        public SubmissionsController(ISubmissionService submissionService, IContestService contestService)
        {
            this.submissionService = submissionService;
            this.contestService = contestService;
        }

        public IActionResult Create()
        {
            if (!this.User.IsAuthenticated)
            {
                return this.RedirectToLogin();
            }

            IEnumerable<string> contests = this.contestService.AllNames().Select(n => n.ToHtml());

            this.ViewModel["contests"] = string.Join(string.Empty, contests);

            return this.View();
        }

        [HttpPost]
        public IActionResult Create(SubmissionModel model)
        {
            if (!this.User.IsAuthenticated)
            {
                return this.RedirectToLogin();
            }

            if (!this.IsValidModel(model))
            {
                this.ShowError(ErrorConstants.SubmissionCodeError);

                return this.View();
            }

            BuildResultType type = BuildResultType.BuildFailed;

            Random random = new Random();
            int randomType = random.Next(0, 101);

            if (randomType > 70)
            {
                type = BuildResultType.BuildSuccess;
            }

            this.submissionService.Create(model.Code, model.Contest, this.Profile.Id, type);

            return this.Redirect("/submissions/all");
        }

        public IActionResult Details(int id)
        {
            if (!this.User.IsAuthenticated)
            {
                return this.RedirectToLogin();
            }

            int userId = this.Profile.Id;

            IEnumerable<ContestByIdModel> contestsListModel = this.contestService.AllContestsById();

            IEnumerable<string> contestsList = contestsListModel.Select(s => s.Id.ToHtml(s.Name));
            IEnumerable<string> submissionsList = this.submissionService.GetContestSubmissionsTypes(id, userId).Select(s => s.Type.ToHtml());

            this.ViewModel["contests"] = string.Join(string.Empty, contestsList);
            this.ViewModel["submissions"] = string.Join(string.Empty, submissionsList);

            return this.View();
        }

        public IActionResult All()
        {
            if (!this.User.IsAuthenticated)
            {
                return this.RedirectToLogin();
            }

            IEnumerable<ContestByIdModel> contestsListModel = this.contestService.AllContestsById();

            IEnumerable<string> contestsList = contestsListModel.Select(s => s.Id.ToHtml(s.Name));

            this.ViewModel["contests"] = string.Join(string.Empty, contestsList);
            this.ViewModel["submissions"] = string.Empty;

            return this.View();
        }
    }
}