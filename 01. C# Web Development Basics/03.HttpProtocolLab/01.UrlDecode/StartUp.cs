namespace _01.UrlDecode
{
    using System;
    using System.Net;

    public class StartUp
    {
        public static void Main()
        {
            string url = Console.ReadLine();

            string decodedUrl = WebUtility.UrlDecode(url);

            Console.WriteLine(decodedUrl);
        }
    }
}