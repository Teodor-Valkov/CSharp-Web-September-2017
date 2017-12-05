namespace CameraBazaar.Services.Models.Identity
{
    using Data.Common;
    using System.ComponentModel.DataAnnotations;

    public class IdentityEditServiceModel
    {
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(PropertyValidationConstants.EmailMaxLength, MinimumLength = PropertyValidationConstants.EmailMinLength, ErrorMessage = PropertyErrorMessageConstants.EmailErrorMessage)]
        public string Email { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("Password", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [RegularExpression(@"^\+\d{10,12}$", ErrorMessage = PropertyErrorMessageConstants.PhoneErrorMessage)]
        public string Phone { get; set; }
    }
}