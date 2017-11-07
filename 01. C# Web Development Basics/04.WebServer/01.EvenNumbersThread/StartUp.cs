namespace _01.EvenNumbersThread
{
    using System;
    using System.Linq;
    using System.Threading;

    public class StartUp
    {
        public static void Main()
        {
            int[] numbers = Console.ReadLine().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
            int start = numbers[0];
            int end = numbers[1];

            Thread evens = new Thread(() =>
            {
                PrintEvenNumbers(start, end);
            });

            evens.Start();
            evens.Join();

            Console.WriteLine("Thread finished work!");
        }

        private static void PrintEvenNumbers(int start, int end)
        {
            for (int i = start; i < end; i++)
            {
                if (i % 2 == 0)
                {
                    Console.WriteLine(i);
                }
            }
        }
    }
}