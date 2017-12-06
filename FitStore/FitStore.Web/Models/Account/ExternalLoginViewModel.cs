namespace FitStore.Web.Models.Account
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using static Data.DataConstants;

    public class ExternalLoginViewModel
    {
        [Display(Name = "Full Name")]
        [MinLength(UserFullNameMinLength, ErrorMessage = "The {0} must be at least {1} symbols long.")]
        [MaxLength(UserFullNameMaxLength, ErrorMessage = "The {0} must be less than {1} characters long.")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        [MinLength(UserEmailMinLength, ErrorMessage = "The {0} must be at least {1} symbols long.")]
        [MaxLength(UserEmailMaxLength, ErrorMessage = "The {0} must be less than {1} characters long.")]
        public string Email { get; set; }

        [MinLength(UserAddressMinLength, ErrorMessage = "The {0} must be at least {1} symbols long.")]
        [MaxLength(UserAddressMaxLength, ErrorMessage = "The {0} must be less than {1} characters long.")]
        public string Address { get; set; }

        [Display(Name = "Birth Date")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
    }
}