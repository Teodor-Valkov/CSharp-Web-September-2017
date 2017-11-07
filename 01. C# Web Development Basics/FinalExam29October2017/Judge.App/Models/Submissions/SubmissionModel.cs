namespace Judge.App.Models.Submissions
{
    using Data.Models;
    using Infrastructure.Mapping;
    using Infrastructure.Validation;

    public class SubmissionModel : IMapFrom<Submission>
    {
        [Required]
        public string Code { get; set; }

        [Required]
        public string Contest { get; set; }
    }
}