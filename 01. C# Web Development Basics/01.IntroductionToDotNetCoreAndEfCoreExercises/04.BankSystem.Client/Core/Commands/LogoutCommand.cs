namespace _04.BankSystem.Client.Core.Commands
{
    using _04.BankSystem.Data;
    using _04.BankSystem.Models;
    using System;

    public class LogoutCommand : Command
    {
        public LogoutCommand(BankSystemDbContext database, string[] commandArgs)
            : base(database, commandArgs)
        {
        }

        public override string Execute()
        {
            if (!AuthenticationManager.IsAuthenticated())
            {
                throw new InvalidOperationException("You should log in first!");
            }

            User user = AuthenticationManager.GetCurrentUser();
            AuthenticationManager.Logout();

            return $"User {user.Username} successfully logged out!";
        }
    }
}