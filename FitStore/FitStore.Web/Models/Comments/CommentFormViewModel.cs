namespace FitStore.Web.Models.Comments
{
    using System.ComponentModel.DataAnnotations;

    using static Common.CommonMessages;
    using static Data.DataConstants;

    public class CommentFormViewModel
    {
        [Required]
        [StringLength(CommentContentMaxLength, ErrorMessage = FieldLengthErrorMessage, MinimumLength = CommentContentMinLength)]
        public string Content { get; set; }

        public int SupplementId { get; set; }
    }
}