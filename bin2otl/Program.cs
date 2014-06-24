using System;
using System.IO;
using dat2otl.Properties;

namespace dat2otl
{
    class Program
    {
        static void Main(string[] args)
        {
            const int minimumFileLength = 65*1024;

            string inputFileName = null;
            if (args.Length >= 1) inputFileName = args[0];
            else
            {
                Console.WriteLine(@"Укажите путь к входному файлу");
                inputFileName = Console.ReadLine();
            }

            string outputFilePath = Path.GetFileNameWithoutExtension(inputFileName) + ".otl";

            using (
                FileStream inputStream = new FileStream(inputFileName, FileMode.Open),
                           outputStream = new FileStream(outputFilePath, FileMode.Create)
                )
            {
                var headerStream = new MemoryStream(Resources.header);
                headerStream.CopyTo(outputStream);
                inputStream.CopyTo(outputStream);
                while (outputStream.Length < minimumFileLength) outputStream.WriteByte(0xff);
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Готово");
            Console.ResetColor();
            Console.ReadLine();
        }
    }
}
