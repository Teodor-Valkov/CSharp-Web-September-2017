namespace GameStore.GameStoreApplication.ViewModels.Account
{
    using GameStore.GameStoreApplication.Common;
    using GameStore.GameStoreApplication.Utilities;
    using System.ComponentModel.DataAnnotations;

    public class RegisterViewModel
    {
        [Required]
        [Email]
        [MaxLength(ValidationConstants.Account.EmailMaxLength, ErrorMessage = ValidationConstants.InvalidMaxLengthErrorMessage)]
        public string Email { get; set; }

        [Display(Name = "Full Name")]
        [MinLength(ValidationConstants.Account.NameMinLength, ErrorMessage = ValidationConstants.InvalidMinLengthErrorMessage)]
        [MaxLength(ValidationConstants.Account.NameMaxLength, ErrorMessage = ValidationConstants.InvalidMaxLengthErrorMessage)]
        public string FullName { get; set; }

        [Required]
        [Password]
        [MinLength(ValidationConstants.Account.PasswordMinLength, ErrorMessage = ValidationConstants.InvalidMinLengthErrorMessage)]
        [MaxLength(ValidationConstants.Account.PasswordMaxLength, ErrorMessage = ValidationConstants.InvalidMaxLengthErrorMessage)]
        public string Password { get; set; }

        [Display(Name = "Confirm Password")]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}