namespace FitStore.Web.Models.Account
{
    using System.ComponentModel.DataAnnotations;

    using static Common.CommonMessages;
    using static Data.DataConstants;

    public class LoginViewModel
    {
        [Required]
        [StringLength(UserUsernameMaxLength, ErrorMessage = FieldLengthErrorMessage, MinimumLength = UserUsernameMinLength)]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(UserPasswordMaxLength, ErrorMessage = FieldLengthErrorMessage, MinimumLength = UserPasswordMinLength)]
        public string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}