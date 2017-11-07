namespace _02.SliceFile
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public class StartUp
    {
        private const string SongPath = "50 Cent - In da club.mp3";
        private const string SlicedSongPath = "Sliced";
        private const string EnterDesiredParts = "Please enter on how many parts do you want to slice the song: ";

        public static void Main()
        {
            string sourcePath = SongPath;
            string destinationPath = SlicedSongPath;

            Console.Write(EnterDesiredParts);
            int parts = int.Parse(Console.ReadLine());

            SliceAsync(sourcePath, destinationPath, parts);

            Console.WriteLine("Anything else?");

            while (true)
            {
                string input = Console.ReadLine();

                if (input == "Exit")
                {
                    break;
                }
            }
        }

        private static void SliceAsync(string sourcePath, string destinationPath, int parts)
        {
            Task.Run(() =>
            {
                Slice(sourcePath, destinationPath, parts);
            });
        }

        private static void Slice(string sourcePath, string destinationPath, int parts)
        {
            if (!Directory.Exists(destinationPath))
            {
                Directory.CreateDirectory(destinationPath);
            }

            using (FileStream source = new FileStream(sourcePath, FileMode.Open))
            {
                FileInfo fileInfo = new FileInfo(sourcePath);

                long partLength = (source.Length / parts) + 1;
                long currentByte = 0;

                for (int currentPart = 1; currentPart <= parts; currentPart++)
                {
                    string filePath = string.Format("{0}/Part-{1}{2}", destinationPath, currentPart, fileInfo.Extension);

                    using (FileStream destination = new FileStream(filePath, FileMode.Create))
                    {
                        byte[] buffer = new byte[1024];

                        while (currentByte <= currentPart * partLength)
                        {
                            int readBytesCount = source.Read(buffer, 0, buffer.Length);

                            if (readBytesCount == 0)
                            {
                                break;
                            }

                            destination.Write(buffer, 0, readBytesCount);
                            currentByte += readBytesCount;
                        }
                    }
                }

                Console.WriteLine("Slicing finished!");
            }
        }
    }
}