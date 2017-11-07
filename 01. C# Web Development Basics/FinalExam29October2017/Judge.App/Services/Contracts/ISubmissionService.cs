namespace Judge.App.Services.Contracts
{
    using Data.Models.Enums;
    using Models.Submissions;
    using System.Collections.Generic;

    public interface ISubmissionService
    {
        void Create(string code, string contest, int userId, BuildResultType type);

        IEnumerable<SubmissionBuildResultTypeModel> GetContestSubmissionsTypes(int contestId, int userId);
    }
}