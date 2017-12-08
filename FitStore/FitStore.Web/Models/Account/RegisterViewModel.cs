namespace FitStore.Web.Models.Account
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using static Common.CommonConstants;
    using static Data.DataConstants;

    public class RegisterViewModel
    {
        [Required]
        [MinLength(UserUsernameMinLength, ErrorMessage = "The {0} must be at least {1} symbols long.")]
        [MaxLength(UserUsernameMaxLength, ErrorMessage = "The {0} must be less than {1} characters long.")]
        public string Username { get; set; }

        [MinLength(UserFullNameMinLength, ErrorMessage = "The {0} must be at least {1} symbols long.")]
        [MaxLength(UserFullNameMaxLength, ErrorMessage = "The {0} must be less than {1} characters long.")]
        [Display(Name = UserFullNameName)]
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

        [Compare(nameof(Password), ErrorMessage = "The password and confirmation password do not match.")]
        [DataType(DataType.Password)]
        [Display(Name = UserConfirmPasswordName)]
        public string ConfirmPassword { get; set; }

        [MinLength(UserPhoneNumberMinLength, ErrorMessage = "The {0} must be at least {1} symbols long.")]
        [MaxLength(UserPhoneNumberMaxLength, ErrorMessage = "The {0} must be less than {1} characters long.")]
        [Display(Name = UserPhoneNumberName)]
        public string PhoneNumber { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = UserBirthDateName)]
        public DateTime BirthDate { get; set; }
    }
}