namespace _04.BankSystem.Models
{
    using System.ComponentModel.DataAnnotations;

    public class CheckingAccount
    {
        public CheckingAccount(string accountNumber, decimal balance, decimal fee)
        {
            this.AccountNumber = accountNumber;
            this.Balance = balance;
            this.Fee = fee;
        }

        public CheckingAccount()
        {
        }

        public int Id { get; set; }

        [Required]
        public string AccountNumber { get; set; }

        public decimal Balance { get; set; }

        public decimal Fee { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }
    }
}