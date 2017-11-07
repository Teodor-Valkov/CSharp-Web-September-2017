namespace _03.RequestParser
{
    using System;
    using System.Collections.Generic;

    public class StartUp
    {
        private const int ResponseNotFoundCode = 404;
        private const string ResponseNotFoundMessage = "Not Found";
        private const int ResponseStatusOkCode = 200;
        private const string ResponseStatusOkMessage = "OK";

        public static void Main()
        {
            Dictionary<string, HashSet<string>> validUrls = new Dictionary<string, HashSet<string>>();

            string input = string.Empty;
            while ((input = Console.ReadLine()) != "END")
            {
                string[] tokens = input.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

                string path = $"/{tokens[0]}";
                string method = tokens[1];

                if (!validUrls.ContainsKey(path))
                {
                    validUrls[path] = new HashSet<string>();
                }

                validUrls[path].Add(method);
            }

            string request = Console.ReadLine();

            string[] requestParts = request.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            string requestMethod = requestParts[0].ToLower();
            string requestPath = requestParts[1];
            string requestProtocol = requestParts[2];

            if (validUrls.ContainsKey(requestPath) && validUrls[requestPath].Contains(requestMethod))
            {
                Console.WriteLine($"{requestProtocol} {ResponseStatusOkCode} {ResponseStatusOkMessage}");
                Console.WriteLine($"Content-Length: {ResponseStatusOkMessage.Length}");
                Console.WriteLine($"Content-Type: text/plain");
                Console.WriteLine($"{Environment.NewLine}{ResponseStatusOkMessage}");
            }
            else
            {
                Console.WriteLine($"{requestProtocol} {ResponseNotFoundCode} {ResponseNotFoundMessage}");
                Console.WriteLine($"Content-Length: {ResponseNotFoundMessage.Length}");
                Console.WriteLine($"Content-Type: text/plain");
                Console.WriteLine($"{Environment.NewLine}{ResponseNotFoundMessage}");
            }
        }
    }
}