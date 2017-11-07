namespace _04.BankSystem.Client.Core.Commands
{
    using _04.BankSystem.Data;
    using _04.BankSystem.Models;
    using System;
    using System.Text;

    public class ListAccountsCommand : Command
    {
        public ListAccountsCommand(BankSystemDbContext database, string[] commandArgs)
            : base(database, commandArgs)
        {
        }

        public override string Execute()
        {
            if (this.CommandArgs.Length != 0)
            {
                throw new ArgumentException("Input is not valid!");
            }

            if (!AuthenticationManager.IsAuthenticated())
            {
                throw new InvalidOperationException("You should log in first!");
            }

            StringBuilder builder = new StringBuilder();

            User user = AuthenticationManager.GetCurrentUser();

            builder.AppendLine("Saving Accounts:");
            foreach (SavingsAccount userSavingAccount in user.SavingAccounts)
            {
                builder.AppendLine($"--{userSavingAccount.AccountNumber} {userSavingAccount.Balance:F2}");
            }

            builder.AppendLine("Checking Accounts:");
            foreach (CheckingAccount checkingAccount in user.CheckingAccounts)
            {
                builder.AppendLine($"--{checkingAccount.AccountNumber} {checkingAccount.Balance:F2}");
            }

            return builder.ToString().Trim();
        }
    }
}