namespace GameStore.App.Infrastructure
{
    public static class TextTransformer
    {
        private const int MaxLength = 300;

        public static string Cut(string text)
        {
            if (text.Length > MaxLength)
            {
                return text.Substring(0, 300) + "...";
            }

            return text;
        }
    }
}