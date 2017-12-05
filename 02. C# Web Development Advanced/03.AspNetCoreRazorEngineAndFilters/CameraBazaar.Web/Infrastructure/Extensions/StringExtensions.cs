namespace CameraBazaar.Web.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        private const string format = "F2";

        public static string ToPrice(this decimal priceAsString)
        {
            return $"${priceAsString.ToString(format)}";
        }
    }
}