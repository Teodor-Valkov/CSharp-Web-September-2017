namespace _04.BankSystem.Models
{
    using System.ComponentModel.DataAnnotations;

    public class SavingsAccount
    {
        public SavingsAccount(string accountNumber, decimal balance, decimal interestRate)
        {
            this.AccountNumber = accountNumber;
            this.Balance = balance;
            this.InterestRate = interestRate;
        }

        public SavingsAccount()
        {
        }

        public int Id { get; set; }

        [Required]
        public string AccountNumber { get; set; }

        public decimal Balance { get; set; }

        public decimal InterestRate { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }
    }
}