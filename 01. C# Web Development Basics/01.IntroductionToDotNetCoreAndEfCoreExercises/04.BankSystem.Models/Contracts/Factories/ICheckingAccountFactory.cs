namespace _04.BankSystem.Models.Contracts.Factories
{
    public interface ICheckingAccountFactory
    {
        CheckingAccount GenerateCheckingAccount(string[] accountArgs);
    }
}