namespace FitStore.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using static DataConstants;

    public class Review
    {
        public int Id { get; set; }

        [Required]
        [MinLength(ReviewContentMinLength)]
        [MaxLength(ReviewContentMaxLength)]
        public string Content { get; set; }

        [Range(1, 10)]
        public int Rating { get; set; }

        public DateTime PublishDate { get; set; }

        public bool IsDeleted { get; set; }

        [Required]
        public string AuthorId { get; set; }

        public User Author { get; set; }

        public int SupplementId { get; set; }

        public Supplement Supplement { get; set; }
    }
}