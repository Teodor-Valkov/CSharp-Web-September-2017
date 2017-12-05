namespace LearningSystem.Services.Models.Users
{
    using Common.Mapping;
    using Data.Models;
    using System;
    using System.ComponentModel.DataAnnotations;

    using static Data.DataConstants;

    public class UserEditProfileServiceModel : IMapFrom<User>
    {
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(UserEmailMaxLength, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = UserEmailMinLength)]
        public string Email { get; set; }

        [Required]
        [StringLength(UserNameMaxLength, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = UserNameMinLength)]
        public string Name { get; set; }

        [DataType(DataType.Date)]
        public DateTime Birthdate { get; set; }
    }
}