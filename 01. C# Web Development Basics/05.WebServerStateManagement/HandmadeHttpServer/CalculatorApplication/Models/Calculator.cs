namespace HandmadeHttpServer.CalculatorApplication.Models
{
    using HandmadeHttpServer.CalculatorApplication.Contracts;
    using HandmadeHttpServer.CalculatorApplication.Strategies;
    using System.Collections.Generic;

    internal class Calculator
    {
        private IStrategy strategy;
        private IDictionary<char, IStrategy> strategies;

        public Calculator(char mathOperator)
        {
            InitializeStrategies();
            this.strategy = this.strategies[mathOperator];
        }

        private void InitializeStrategies()
        {
            this.strategies = new Dictionary<char, IStrategy>
            {
                { '+', new AdditionStrategy()},
                { '-', new SubtractionStrategy()},
                { '*', new MultiplicationStrategy()},
                { '/', new DivisionStrategy()},
            };
        }

        public int Calculate(int firstNumber, int secondNumber)
        {
            return this.strategy.Calculate(firstNumber, secondNumber);
        }
    }
}