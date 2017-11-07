namespace _04.BankSystem.Client.Core.Commands
{
    using _04.BankSystem.Data;
    using _04.BankSystem.Models;
    using _04.BankSystem.Models.Contracts.Validators;
    using _04.BankSystem.Models.Validators;
    using System;
    using System.Linq;

    public class RegisterCommand : Command
    {
        public RegisterCommand(BankSystemDbContext database, string[] commandArgs)
            : base(database, commandArgs)
        {
        }

        public override string Execute()
        {
            if (this.CommandArgs.Length != 3)
            {
                throw new ArgumentException("Input is not valid!");
            }

            if (AuthenticationManager.IsAuthenticated())
            {
                throw new InvalidOperationException("You should log out before log in with another user!");
            }

            string username = this.CommandArgs[0];
            string password = this.CommandArgs[1];
            string email = this.CommandArgs[2];

            User user = new User()
            {
                Username = username,
                Password = password,
                Email = email
            };

            IValidationResult validationResult = UserValidator.IsValid(user);

            if (!validationResult.IsValid)
            {
                throw new ArgumentException(string.Join($"{Environment.NewLine}", validationResult.ValidationErrors));
            }

            if (this.Database.Users.Any(u => u.Username == username))
            {
                throw new ArgumentException("Username already taken!");
            }

            if (this.Database.Users.Any(u => u.Email == email))
            {
                throw new ArgumentException("Email already taken!");
            }

            this.Database.Users.Add(user);
            this.Database.SaveChanges();

            return $"User {username} registered successfully!";
        }
    }
}