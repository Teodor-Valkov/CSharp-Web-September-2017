namespace ModPanel.App.Infrastructure.Helpers
{
    using System.Linq;

    public static class StringExtensions
    {
        public static string Capitalize(this string input)
        {
            if (input == null || !input.Any())
            {
                return input;
            }

            char firstLetter = char.ToUpper(input.First());
            string restLetters = input.Substring(1);

            return $"{firstLetter}{restLetters}";
        }
    }
}