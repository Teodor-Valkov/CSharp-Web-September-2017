namespace Judge.App.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        [MaxLength(100)]
        public string Password { get; set; }

        [MaxLength(100)]
        public string FullName { get; set; }

        public bool IsAdmin { get; set; }

        public IList<Contest> Contests { get; set; } = new List<Contest>();

        public IList<Submission> Submissions { get; set; } = new List<Submission>();
    }
}