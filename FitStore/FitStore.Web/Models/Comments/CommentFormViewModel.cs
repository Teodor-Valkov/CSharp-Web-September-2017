namespace FitStore.Web.Models.Comments
{
    using System.ComponentModel.DataAnnotations;

    using static Data.DataConstants;

    public class CommentFormViewModel
    {
        [Required]
        [MinLength(CommentContentMinLength)]
        [MaxLength(CommentContentMaxLength)]
        public string Content { get; set; }
    }
}