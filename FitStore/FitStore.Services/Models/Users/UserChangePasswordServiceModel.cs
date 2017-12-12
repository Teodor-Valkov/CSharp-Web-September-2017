namespace FitStore.Services.Models.Users
{
    using Common.Mapping;
    using Data.Models;
    using System.ComponentModel.DataAnnotations;

    using static Common.CommonConstants;
    using static Data.DataConstants;

    public class UserChangePasswordServiceModel : IMapFrom<User>
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = UserCurrentPasswordName)]
        public string OldPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(UserPasswordMinLength, ErrorMessage = "The {0} must be at least {1} symbols long.")]
        [MaxLength(UserPasswordMaxLength, ErrorMessage = "The {0} must be less than {1} characters long.")]
        [Display(Name = UserPasswordName)]
        public string NewPassword { get; set; }

        [Compare(nameof(NewPassword), ErrorMessage = "The password and confirmation password do not match.")]
        [DataType(DataType.Password)]
        [Display(Name = UserConfirmPasswordName)]
        public string ConfirmPassword { get; set; }
    }
}