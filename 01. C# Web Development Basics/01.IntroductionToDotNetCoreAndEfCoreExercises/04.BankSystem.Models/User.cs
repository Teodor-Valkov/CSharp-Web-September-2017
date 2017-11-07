namespace _04.BankSystem.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Email { get; set; }

        public ICollection<SavingsAccount> SavingAccounts { get; set; } = new List<SavingsAccount>();

        public ICollection<CheckingAccount> CheckingAccounts { get; set; } = new List<CheckingAccount>();
    }
}