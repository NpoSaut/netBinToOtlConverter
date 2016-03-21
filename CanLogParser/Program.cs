﻿using System;
using BlokFrames;
using Communications.Can;

namespace CanLogParser
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            FormatManager formatManager =
                new FormatManager()
                    .AddFormatter<SautReadyA>(f => string.Format("Переключение САУТ: {0}", f.Track))
                    .AddFormatter<InputData>(f => f.Index == 1 ? string.Format("Запрос на переключение пути: {0}", GetTrackName(f.Data)) : null)
                    .AddFormatter<SysData>(f => f.Index == 1 ? string.Format("Подтверждение переключения пути: {0}", GetTrackName(f.Data)) : null)
                    .AddFormatter<SautPtkReady2A1>(
                        f => string.Format("{0} перегон; генератор: {1} ({2}); маршрут: {3}", f.SpanNumber, f.GeneratorNumber, f.GeneratorKind, f.RouteNumber));

            DateTime date = DateTime.MinValue;
            ICanLogSource logSource = new TextCanLogSource("input.txt");
            foreach (CanFrame frame in logSource.ReadFrames())
            {
                try
                {
                    BlokFrame blokFrame = BlokFrame.GetBlokFrame(frame);
                    if (blokFrame is IpdDate)
                        date = ((IpdDate)blokFrame).Time;
                    if (date != DateTime.MinValue)
                    {
                        string s = formatManager.FormatString(blokFrame);
                        if (s != null)
                            Console.WriteLine("{0:T}   {1}", date, s);
                    }
                }
                catch (ApplicationException) { }
            }
            Console.ReadLine();
        }

        private static string GetTrackName(int TrackCode)
        {
            return TrackCode < 16
                       ? string.Format("{0} ПР", TrackCode)
                       : string.Format("{0} НПР", TrackCode - 15);
        }
    }
}
