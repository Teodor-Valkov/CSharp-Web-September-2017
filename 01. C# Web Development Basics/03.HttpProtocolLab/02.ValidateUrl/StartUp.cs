namespace _02.ValidateUrl
{
    using System;
    using System.Net;
    using System.Text.RegularExpressions;

    public class StartUp
    {
        private const string InvalidUrl = "Invalid URL!";

        public static void Main()
        {
            string url = Console.ReadLine();
            string decodedUrl = WebUtility.UrlDecode(url);

            Uri uri = new Uri(decodedUrl);

            if (string.IsNullOrEmpty(uri.Scheme) || string.IsNullOrEmpty(uri.Host) || string.IsNullOrEmpty(uri.AbsolutePath) || uri.Port < 0)
            {
                Console.WriteLine(InvalidUrl);
                return;
            }

            Regex regex = new Regex(@"(https|http):\/{2}([a-zA-Z0-9\.-]+)(\:\d+)?([\/][\w\W]+)?");
            if (!regex.IsMatch(decodedUrl))
            {
                Console.WriteLine(InvalidUrl);
                return;
            }

            Console.WriteLine($"Protocol: {uri.Scheme}");
            Console.WriteLine($"Host: {uri.Host}");
            Console.WriteLine($"Port: {uri.Port}");
            Console.WriteLine($"Path: {uri.AbsolutePath}");

            if (!string.IsNullOrEmpty(uri.Query))
            {
                Console.WriteLine($"Query: {uri.Query.Remove(0, 1)}");
            }
            if (!string.IsNullOrEmpty(uri.Fragment))
            {
                Console.WriteLine($"Fragment: {uri.Fragment.Remove(0, 1)}");
            }
        }
    }
}