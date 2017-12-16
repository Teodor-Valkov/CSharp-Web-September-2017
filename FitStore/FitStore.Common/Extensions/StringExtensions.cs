namespace FitStore.Common.Extensions
{
    using System.IO;
    using System.Text.RegularExpressions;

    public static class StringExtensions
    {
        public static string ToFriendlyUrl(this string text)
        {
            return Regex.Replace(text, @"[^A-Za-z0-9_\.~]+", "-");
        }

        public static byte[] ToByteArray(this string imagePath)
        {
            return File.ReadAllBytes(imagePath);
        }
    }
}