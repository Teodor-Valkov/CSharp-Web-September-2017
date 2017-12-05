namespace CameraBazaar.Services.Models.Users
{
    using Data.Common;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class UserEditServiceModel
    {
        [Required]
        [EmailAddress]
        [StringLength(PropertyValidationConstants.EmailMaxLength, MinimumLength = PropertyValidationConstants.EmailMinLength, ErrorMessage = PropertyErrorMessageConstants.EmailErrorMessage)]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [RegularExpression(@"^\+\d{10,12}$", ErrorMessage = PropertyErrorMessageConstants.PhoneErrorMessage)]
        public string Phone { get; set; }

        public DateTime LastLoginTime { get; set; }
    }
}