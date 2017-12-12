namespace FitStore.Services.Models.Users
{
    using Common.Mapping;
    using Data.Models;
    using System;
    using System.ComponentModel.DataAnnotations;

    using static Common.CommonConstants;
    using static Data.DataConstants;

    public class UserEditProfileServiceModel : IMapFrom<User>
    {
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

        [MinLength(UserPhoneNumberMinLength, ErrorMessage = "The {0} must be at least {1} symbols long.")]
        [MaxLength(UserPhoneNumberMaxLength, ErrorMessage = "The {0} must be less than {1} characters long.")]
        [Display(Name = UserPhoneNumberName)]
        public string PhoneNumber { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = UserBirthDateName)]
        public DateTime BirthDate { get; set; }
    }
}