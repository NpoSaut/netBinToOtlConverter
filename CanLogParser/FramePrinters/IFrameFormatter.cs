using System;
using BlokFrames;

namespace CanLogParser.FramePrinters
{
    public interface IFrameFormatter
    {
        string GetString(BlokFrame Frame);
    }

    public class DelegateFrameFormatter<TFrame> : IFrameFormatter
        where TFrame : BlokFrame
    {
        private readonly Func<TFrame, string> _formatDelegate;
        public DelegateFrameFormatter(Func<TFrame, string> FormatDelegate) { _formatDelegate = FormatDelegate; }
        public string GetString(BlokFrame Frame) { return _formatDelegate((TFrame)Frame); }
    }
}
