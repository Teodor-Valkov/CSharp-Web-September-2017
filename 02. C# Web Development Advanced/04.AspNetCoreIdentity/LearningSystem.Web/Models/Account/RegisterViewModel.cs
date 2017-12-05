namespace LearningSystem.Web.Models.Account
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using static Data.DataConstants;

    public class RegisterViewModel
    {
        [Required]
        [StringLength(UserUsernameMaxLength, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = UserUsernameMinLength)]
        public string Username { get; set; }

        [Required]
        [StringLength(UserNameMaxLength, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = UserNameMinLength)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(UserEmailMaxLength, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = UserEmailMinLength)]
        public string Email { get; set; }

        [Required]
        [StringLength(UserPasswordMaxLength, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = UserPasswordMinLength)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [DataType(DataType.Date)]
        public DateTime Birthdate { get; set; }
    }
}