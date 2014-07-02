using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace RpsExtractor
{
    class Program
    {
        static void Main(string[] args)
        {
            string Pattern = @"(?<descriptor>[0-9a-fA-F]{4})[\s:]+?((?<databyte>[0-9a-fA-F]{2})\s*){1,8}$";
            if (args.Length == 0)
            {
                Console.WriteLine("Запустите программу со следующими параметрами:");
                Console.WriteLine("  1) Путь к текстовому файлу");
                Console.WriteLine("  2) Путь к бинарному файлу");
                Console.WriteLine("  3) Дескриптор");
                return;
            }
            string InPath  = args[0];
            
            string OutPath;
            int TargetDescripter;

            if (args.Length > 1)
            {
                OutPath = args[1];
                TargetDescripter = Convert.ToUInt16(args[2], 16);
            }
            else
            {
                string InDir = System.IO.Path.GetDirectoryName(InPath);
                string InName = System.IO.Path.GetFileNameWithoutExtension(InPath);
                OutPath = System.IO.Path.Combine(InDir, InName + ".otl");
                Console.ResetColor();
                Console.WriteLine("Введите дескриптор (в HEX) или оставьте поле пустым, чтобы выключить фильтр");
                Console.Write("Дескриптор: ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                var descrString = Console.ReadLine();
                if (descrString.Length > 0) TargetDescripter = Convert.ToUInt16(descrString, 16);
                else TargetDescripter = -1;
            }


            var ExePath = (new Uri(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase)).LocalPath;
            var HeaderPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(ExePath), "header.bin");

            Stream HeaderStream = File.Exists(HeaderPath) ? (Stream)new FileStream(HeaderPath, FileMode.Open) : (Stream)new MemoryStream(0);

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Текстовый файл: {0}", InPath);
            Console.WriteLine("Бинарный файл:  {0}", OutPath);
            Console.ForegroundColor = ConsoleColor.Blue;
            if (TargetDescripter != -1)
                Console.WriteLine("Фильтр на дескриптор: {0:X4}", TargetDescripter);

            Regex regex = new Regex(Pattern);

            Console.ResetColor();

            int BytesCounter = 0;

            using (var OutStream = new FileStream(OutPath, FileMode.Create))
            using (var tr = new StreamReader(InPath))
            {
                HeaderStream.CopyTo(OutStream);
                String line;
                while ((line = tr.ReadLine()) != null)
                {
                    Match match = regex.Match(line);
                    if (match.Success)
                    {
                        var desc = Convert.ToUInt16(match.Groups["descriptor"].Value, 16);
                        if (TargetDescripter == -1 || desc == TargetDescripter)
                        {
                            var c = match.Groups["databyte"].Captures.OfType<Capture>().ToList();
                            IList<byte> bs = new List<byte>();
                            foreach (var bc in c)
                            {
                                byte b = Convert.ToByte(bc.Value, 16);
                                OutStream.WriteByte(b);
                                bs.Add(b);
                                BytesCounter++;
                            }
                            Console.Write('.');
                            Debug.WriteLine("{0}  --> descriptor: {1:X4}, data: {2}", line, desc, BitConverter.ToString(bs.ToArray()));
                        }
                        else
                        {
                            Debug.WriteLine("{0}  --X descriptor: {1:X4}, data: {2}", line, desc,
                                            BitConverter.ToString(
                                                match.Groups["databyte"].Captures.OfType<Capture>()
                                                                        .ToList()
                                                                        .Select(bc => Convert.ToByte(bc.Value, 16))
                                                                        .ToArray()));
                        }
                    }
                    else
                        Debug.WriteLine("{0}  XXX", line);
                }
                while (OutStream.Position < 65 * 1024)
                    OutStream.WriteByte(0xff);
            }
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Готово. {0} байт сконвертировано", BytesCounter);

        }
    }
}
