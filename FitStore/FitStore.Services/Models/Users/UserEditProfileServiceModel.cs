namespace FitStore.Services.Models.Users
{
    using Common.Mapping;
    using Data.Models;
    using System;
    using System.ComponentModel.DataAnnotations;

    using static Common.CommonConstants;
    using static Common.CommonMessages;
    using static Data.DataConstants;

    public class UserEditProfileServiceModel : IMapFrom<User>
    {
        [StringLength(UserFullNameMaxLength, ErrorMessage = FieldLengthErrorMessage, MinimumLength = UserFullNameMinLength)]
        [Display(Name = UserFullNameName)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(UserEmailMaxLength, ErrorMessage = FieldLengthErrorMessage, MinimumLength = UserEmailMinLength)]
        public string Email { get; set; }

        [StringLength(UserAddressMaxLength, ErrorMessage = FieldLengthErrorMessage, MinimumLength = UserAddressMinLength)]
        public string Address { get; set; }

        [StringLength(UserPhoneNumberMaxLength, ErrorMessage = FieldLengthErrorMessage, MinimumLength = UserPhoneNumberMinLength)]
        [Display(Name = UserPhoneNumberName)]
        public string PhoneNumber { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = UserBirthDateName)]
        public DateTime BirthDate { get; set; }
    }
}