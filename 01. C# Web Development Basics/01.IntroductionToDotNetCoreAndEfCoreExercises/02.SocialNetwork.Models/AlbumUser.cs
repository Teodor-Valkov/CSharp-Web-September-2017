namespace _02.SocialNetwork.Models
{
    using Enums;

    public class AlbumUser
    {
        public int AlbumId { get; set; }

        public Album Album { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public Role Role { get; set; }
    }
}