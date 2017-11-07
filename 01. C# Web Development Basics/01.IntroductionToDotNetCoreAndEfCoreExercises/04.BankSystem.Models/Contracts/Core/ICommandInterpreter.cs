namespace _04.BankSystem.Models.Contracts.Core
{
    public interface ICommandInterpreter
    {
        string ProcessCommand(string input);
    }
}