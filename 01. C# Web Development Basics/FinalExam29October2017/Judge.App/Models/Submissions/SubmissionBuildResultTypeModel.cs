namespace Judge.App.Models.Submissions
{
    using Data.Models;
    using Infrastructure.Mapping;
    using Judge.App.Data.Models.Enums;

    public class SubmissionBuildResultTypeModel : IMapFrom<Submission>
    {
        public BuildResultType Type { get; set; }
    }
}