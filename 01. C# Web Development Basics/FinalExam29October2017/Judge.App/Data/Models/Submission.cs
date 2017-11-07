namespace Judge.App.Data.Models
{
    using Data.Models.Enums;
    using System.ComponentModel.DataAnnotations;

    public class Submission
    {
        public int Id { get; set; }

        [Required]
        public string Code { get; set; }

        public int ContestId { get; set; }

        public Contest Contest { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public BuildResultType BuildResultType { get; set; }
    }
}