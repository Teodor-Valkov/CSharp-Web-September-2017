namespace FitStore.Web.Models.Account
{
    using System.ComponentModel.DataAnnotations;

    using static Data.DataConstants;

    public class LoginViewModel
    {
        [Required]
        [MinLength(UserUsernameMinLength, ErrorMessage = "The {0} must be at least {1} symbols long.")]
        [MaxLength(UserUsernameMaxLength, ErrorMessage = "The {0} must be less than {1} characters long.")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(UserPasswordMinLength, ErrorMessage = "The {0} must be at least {1} symbols long.")]
        [MaxLength(UserPasswordMaxLength, ErrorMessage = "The {0} must be less than {1} characters long.")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}