namespace _04.BankSystem.Client.Core.Commands
{
    using _04.BankSystem.Data;
    using _04.BankSystem.Models;
    using System;
    using System.Linq;

    public class LoginCommand : Command
    {
        public LoginCommand(BankSystemDbContext database, string[] commandArgs)
            : base(database, commandArgs)
        {
        }

        public override string Execute()
        {
            if (this.CommandArgs.Length != 2)
            {
                throw new ArgumentException("Input is not valid!");
            }

            if (AuthenticationManager.IsAuthenticated())
            {
                throw new InvalidOperationException("You should logout first!");
            }

            string username = this.CommandArgs[0];
            string password = this.CommandArgs[1];

            User user = this.Database.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user == null)
            {
                throw new ArgumentException("Invalid username/password!");
            }

            AuthenticationManager.Login(user);

            return $"User {username} logged in successfully!";
        }
    }
}