namespace BookShop.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using static DataConstants;

    public class Author
    {
        public int Id { get; set; }

        [Required]
        [MinLength(AuthorNameMinLength)]
        [MaxLength(AuthorNameMaxLength)]
        public string FirstName { get; set; }

        [Required]
        [MinLength(AuthorNameMinLength)]
        [MaxLength(AuthorNameMaxLength)]
        public string LastName { get; set; }

        public IList<Book> Books { get; set; } = new List<Book>();
    }
}