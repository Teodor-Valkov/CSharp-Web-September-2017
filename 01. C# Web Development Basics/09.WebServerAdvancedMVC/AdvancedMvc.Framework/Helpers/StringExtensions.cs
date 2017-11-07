namespace AdvancedMvc.Framework.Helpers
{
    using System.Linq;

    public static class StringExtensions
    {
        public static string Capitalize(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            char firstLetter = char.ToUpper(input.First());
            string restLetters = input.Substring(1);

            return $"{firstLetter}{restLetters}";
        }
    }
}