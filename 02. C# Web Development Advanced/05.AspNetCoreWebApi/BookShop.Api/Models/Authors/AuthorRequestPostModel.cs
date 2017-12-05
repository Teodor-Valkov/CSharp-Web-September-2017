namespace BookShop.Api.Models.Authors
{
    using System.ComponentModel.DataAnnotations;

    using static Data.DataConstants;

    public class AuthorRequestPostModel
    {
        [Required]
        [MinLength(AuthorNameMinLength)]
        [MaxLength(AuthorNameMaxLength)]
        public string FirstName { get; set; }

        [Required]
        [MinLength(AuthorNameMinLength)]
        [MaxLength(AuthorNameMaxLength)]
        public string LastName { get; set; }
    }
}