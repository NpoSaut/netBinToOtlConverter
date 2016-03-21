using System;
using BlokFrames;

namespace CanLogParser.FramePrinters
{
    public abstract class FrameFormatterBase<TFrame> : IFrameFormatter
        where TFrame : BlokFrame
    {
        public string GetString(BlokFrame Frame) { return GetStringImpl((TFrame)Frame); }

        protected abstract string GetStringImpl(TFrame Frame);
    }

    public class InputDataFrameFormatter : FrameFormatterBase<InputData>
    {
        protected override string GetStringImpl(InputData Frame)
        {
            throw new NotImplementedException();
            if (Frame.Index != 1)
                return null;
        }
    }
}
