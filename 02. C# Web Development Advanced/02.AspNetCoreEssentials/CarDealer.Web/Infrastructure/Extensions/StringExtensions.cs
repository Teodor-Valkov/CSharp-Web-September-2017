namespace CarDealer.Web.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        private const string format = "F2";

        public static string ToPrice(this decimal priceAsString)
        {
            return $"${priceAsString.ToString(format)}";
        }

        public static string ToPercentage(this double percentageAsString)
        {
            return $"{percentageAsString.ToString(format)}%";
        }
    }
}