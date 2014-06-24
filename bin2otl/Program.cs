using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using dat2otl.Properties;

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
                Console.WriteLine(@"Укажите путь к входному файлу");
                inputFileName = Console.ReadLine();
            }

            string outputFilePath = Path.Combine(Path.GetDirectoryName(inputFileName), "1.otl");

            using (
                Stream inputStream = new FileStream(inputFileName, FileMode.Open),
                    outputStream = new FileStream(outputFilePath, FileMode.Create),
                    headerStream = new MemoryStream(Properties.Resources.header)
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

        private static readonly Byte[] ByteSample = new byte[] { 0xff, 0xaa, 0xce };
        private const int LineAlignLength = 64;

        public static Byte[] ProcessData(Byte[] Data)
        {
            var outputStream = new MemoryStream();
            int lastLineLength = 0;
            for (int i = 0; i < Data.Length; i++)
            {
                if (Data.Skip(i).Take(ByteSample.Length).SequenceEqual(ByteSample))
                {
                    //int bytesToAlign = (LineAlignLength - lastLineLength % LineAlignLength) % LineAlignLength;
                    int bytesToAlign = LineAlignLength * (int)Math.Ceiling((double)lastLineLength / LineAlignLength) - lastLineLength;
                    foreach (var b in Enumerable.Repeat((Byte)0xFF, bytesToAlign)) outputStream.WriteByte(b);
                }
                outputStream.WriteByte(Data[i]);
                lastLineLength++;
            }
            return outputStream.ToArray();
        }

    }
}
