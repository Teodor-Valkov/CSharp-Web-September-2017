namespace _04.BankSystem.Models.Contracts.Factories
{
    public interface ISavingsAccountFactory
    {
        SavingsAccount GenerateSavingAccount(string[] accountArgs);
    }
}