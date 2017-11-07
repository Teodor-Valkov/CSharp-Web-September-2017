namespace _04.BankSystem.Client.Core.Commands
{
    using _04.BankSystem.Data;
    using _04.BankSystem.Models;
    using System;
    using System.Linq;

    public class AddInterestCommand : Command
    {
        public AddInterestCommand(BankSystemDbContext database, string[] commandArgs)
            : base(database, commandArgs)
        {
        }

        public override string Execute()
        {
            if (this.CommandArgs.Length != 1)
            {
                throw new ArgumentException("Input is not valid!");
            }

            if (!AuthenticationManager.IsAuthenticated())
            {
                throw new InvalidOperationException("You should log in first!");
            }

            string accountNumber = this.CommandArgs[0];

            User user = AuthenticationManager.GetCurrentUser();
            SavingsAccount savingsAccount = user.SavingAccounts.FirstOrDefault(a => a.AccountNumber == accountNumber);

            if (savingsAccount == null)
            {
                throw new ArgumentException($"Account {accountNumber} does not exist!");
            }

            savingsAccount.Balance += savingsAccount.Balance * savingsAccount.InterestRate;

            this.Database.SaveChanges();

            return $"Account interest added {accountNumber} - ${savingsAccount.Balance:F2}";
        }
    }
}