using System.Collections.Generic;
using Communications.Can;

namespace CanLogParser
{
    public interface ICanLogSource
    {
        IEnumerable<CanFrame> ReadFrames();
    }
}