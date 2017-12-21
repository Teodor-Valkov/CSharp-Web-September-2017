namespace FitStore.Common.Extensions
{
    using System.IO;

    public static class StringExtensions
    {
        public static byte[] ToByteArray(this string imagePath)
        {
            return File.ReadAllBytes(imagePath);
        }
    }
}