namespace _02.SocialNetwork.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Album
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string BackgroundColor { get; set; }

        public bool IsPublic { get; set; }

        public int CreatorId { get; set; }

        public User Creator { get; set; }

        public ICollection<AlbumUser> SharedAlbums { get; set; } = new List<AlbumUser>();

        public ICollection<AlbumPicture> Pictures { get; set; } = new List<AlbumPicture>();

        public ICollection<AlbumTag> Tags { get; set; } = new List<AlbumTag>();
    }
}