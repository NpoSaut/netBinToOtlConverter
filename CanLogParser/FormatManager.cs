using System;
using System.Collections.Generic;
using BlokFrames;
using CanLogParser.FramePrinters;

namespace CanLogParser
{
    public class FormatManager
    {
        private readonly IDictionary<Type, IFrameFormatter> _formatters;

        public FormatManager() { _formatters = new Dictionary<Type, IFrameFormatter>(); }

        public FormatManager AddFormatter<TFrame>(Func<TFrame, string> Formatter) where TFrame : BlokFrame
        {
            _formatters.Add(typeof (TFrame), new DelegateFrameFormatter<TFrame>(Formatter));
            return this;
        }

        public string FormatString(BlokFrame Frame)
        {
            IFrameFormatter formatter;
            return _formatters.TryGetValue(Frame.GetType(), out formatter)
                       ? formatter.GetString(Frame)
                       : null;
        }
    }
}
