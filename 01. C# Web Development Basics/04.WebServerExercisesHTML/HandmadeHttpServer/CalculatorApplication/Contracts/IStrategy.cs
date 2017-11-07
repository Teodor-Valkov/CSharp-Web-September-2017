namespace HandmadeHttpServer.CalculatorApplication.Contracts
{
    public interface IStrategy
    {
        int Calculate(int firstNumber, int secondNumber);
    }
}