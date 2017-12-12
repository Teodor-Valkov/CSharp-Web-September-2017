namespace FitStore.Services.Models.Users
{
    using Common.Mapping;
    using Data.Models;
    using System;
    using System.ComponentModel.DataAnnotations;

    using static Common.CommonConstants;

    public class UserProfileServiceModel : IMapFrom<User>
    {
        public string Username { get; set; }

        [Display(Name = UserFullNameName)]
        public string FullName { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        [Display(Name = UserPhoneNumberName)]
        public string PhoneNumber { get; set; }

        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
    }
}