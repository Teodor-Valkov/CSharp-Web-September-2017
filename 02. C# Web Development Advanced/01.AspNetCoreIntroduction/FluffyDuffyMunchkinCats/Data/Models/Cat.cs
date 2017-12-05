using System.ComponentModel.DataAnnotations;

namespace FluffyDuffyMunchkinCats.Data.Models
{
    public class Cat
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Range(0, 30)]
        public int Age { get; set; }

        [Required]
        [MaxLength(50)]
        public string Breed { get; set; }

        [Required]
        [MaxLength(2000)]
        public string ImageUrl { get; set; }
    }
}