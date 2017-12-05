namespace FitStore.Web.Models.Account
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using static Data.DataConstants;

    public class RegisterViewModel
    {
        [Required]
        [MinLength(UserUsernameMinLength, ErrorMessage = "The {0} must be at least {1} symbols long.")]
        [MaxLength(UserUsernameMaxLength, ErrorMessage = "The {0} must be less than {1} characters long.")]
        public string Username { get; set; }

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

        [Required]
        [DataType(DataType.Password)]
        [MinLength(UserPasswordMinLength, ErrorMessage = "The {0} must be at least {1} symbols long.")]
        [MaxLength(UserPasswordMaxLength, ErrorMessage = "The {0} must be less than {1} characters long.")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Birth Date")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
    }
}