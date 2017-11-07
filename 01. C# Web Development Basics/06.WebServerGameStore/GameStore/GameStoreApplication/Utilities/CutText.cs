namespace GameStore.GameStoreApplication.Utilities
{
    public static class CutText
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