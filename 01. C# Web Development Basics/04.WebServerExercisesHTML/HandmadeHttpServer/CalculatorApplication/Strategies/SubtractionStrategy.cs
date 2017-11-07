namespace HandmadeHttpServer.CalculatorApplication.Strategies
{
    using HandmadeHttpServer.CalculatorApplication.Contracts;

    public class SubtractionStrategy : IStrategy
    {
        public int Calculate(int firstNumber, int secondNumber)
        {
            return firstNumber - secondNumber;
        }
    }
}