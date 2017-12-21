namespace FitStore.Services.Models.Users
{
    using Common.Mapping;
    using Data.Models;
    using System.ComponentModel.DataAnnotations;

    using static Common.CommonConstants;
    using static Common.CommonMessages;
    using static Data.DataConstants;

    public class UserChangePasswordServiceModel : IMapFrom<User>
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = UserCurrentPasswordName)]
        public string OldPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(UserPasswordMaxLength, ErrorMessage = FieldLengthErrorMessage, MinimumLength = UserPasswordMinLength)]
        [Display(Name = UserPasswordName)]
        public string NewPassword { get; set; }

        [Compare(nameof(NewPassword), ErrorMessage = PasswordsDoNotMatch)]
        [DataType(DataType.Password)]
        [Display(Name = UserConfirmPasswordName)]
        public string ConfirmPassword { get; set; }
    }
}