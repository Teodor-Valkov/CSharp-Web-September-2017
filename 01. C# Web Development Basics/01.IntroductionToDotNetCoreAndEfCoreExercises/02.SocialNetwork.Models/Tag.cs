namespace _02.SocialNetwork.Models
{
    using Attributes;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Tag
    {
        public int Id { get; set; }

        [Required]
        [Tag]
        public string Name { get; set; }

        public ICollection<AlbumTag> Albums { get; set; } = new List<AlbumTag>();
    }
}