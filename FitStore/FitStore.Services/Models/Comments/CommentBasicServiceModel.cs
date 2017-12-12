namespace FitStore.Services.Models.Comments
{
    using Common.Mapping;
    using Data.Models;

    public class CommentBasicServiceModel : IMapFrom<Comment>
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public string AuthorId { get; set; }

        public int SupplementId { get; set; }
    }
}