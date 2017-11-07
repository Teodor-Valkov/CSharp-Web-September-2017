namespace _04.BankSystem.Models.Contracts.IO
{
    public interface IWriter
    {
        void WriteLine(string message);

        void WriteAllText();
    }
}