using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace bin2otl
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
                Console.WriteLine("Укажите путь к входному файлу");
                inputFileName = Console.ReadLine();
            }

            string headerFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "header.bin");
            string outputFilePath = Path.GetFileNameWithoutExtension(inputFileName) + ".otl";

            using (
                FileStream inputStream = new FileStream(inputFileName, FileMode.Open),
                    outputStream = new FileStream(outputFilePath, FileMode.Create),
                    headerStream = new FileStream(headerFilePath, FileMode.Open)
                )
            {
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
