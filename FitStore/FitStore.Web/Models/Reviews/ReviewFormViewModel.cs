namespace FitStore.Web.Models.Reviews
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using static Common.CommonMessages;
    using static Data.DataConstants;

    public class ReviewFormViewModel
    {
        [Required]
        [StringLength(ReviewContentMaxLength, ErrorMessage = FieldLengthErrorMessage, MinimumLength = ReviewContentMinLength)]
        public string Content { get; set; }

        [Range(1, 10)]
        public int Rating { get; set; }

        public IEnumerable<SelectListItem> Ratings { get; set; }
    }
}