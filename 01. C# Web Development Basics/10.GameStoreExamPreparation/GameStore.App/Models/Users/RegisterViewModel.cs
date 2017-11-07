namespace GameStore.App.Models.Users
{
    using Infrastructure.Validation;
    using Infrastructure.Validation.Users;

    public class RegisterViewModel
    {
        [Required]
        [Email]
        public string Email { get; set; }

        [FullName]
        public string FullName { get; set; }

        [Required]
        [Password]
        public string Password { get; set; }

        [Required]
        [Password]
        public string ConfirmPassword { get; set; }
    }
}