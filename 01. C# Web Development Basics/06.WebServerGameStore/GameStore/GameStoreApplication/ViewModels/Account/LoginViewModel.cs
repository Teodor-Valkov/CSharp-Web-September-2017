namespace GameStore.GameStoreApplication.ViewModels.Account
{
    using GameStore.GameStoreApplication.Utilities;
    using System.ComponentModel.DataAnnotations;

    public class LoginViewModel
    {
        [Required]
        [Email]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}