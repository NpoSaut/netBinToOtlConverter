using System;
using System.Collections.Generic;
using System.IO;
using dat2otl.Properties;
using System.Linq;

namespace bin2otl
{
    class Program
    {
        public const int PageSize = 0x10000;    // 65536

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
                       headerStream = new MemoryStream(Resources.header),
                       pageHeaderStream = new MemoryStream(Resources.prom_header)
                )
            {
                headerStream.CopyTo(outputStream);

                while (inputStream.Position < inputStream.Length)
                {
                    byte[] message = ReadMessage(inputStream);
                    if (outputStream.Position % PageSize + message.Length < PageSize)
                        outputStream.Write(message, 0, message.Length);
                    else
                    {
                        StuffPage(outputStream);
                        pageHeaderStream.Seek(0, SeekOrigin.Begin);
                        pageHeaderStream.CopyTo(outputStream);
                        outputStream.Write(message, 0, message.Length);
                    }
                }

                StuffPage(outputStream);
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Готово");
            Console.ResetColor();
            Console.ReadLine();
        }

        private static void StuffPage(Stream stuffingStream) { while (stuffingStream.Length % PageSize != 0) stuffingStream.WriteByte(0xff); }

        public static Byte[] ReadMessage(Stream InputStream)
        {
            var res = new List<Byte>();
            int b;
            do
            {
                b = InputStream.ReadByte();
                if (b == -1) break;
                else res.Add((byte)b);
            } while (b != 0xFF);
            return res.ToArray();
        }
    }
}
