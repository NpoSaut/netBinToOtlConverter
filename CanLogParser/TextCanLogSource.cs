using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Communications.Can;

namespace CanLogParser
{
    public class TextCanLogSource : ICanLogSource
    {
        private const string Pattern = @"(?<descriptor>[0-9a-fA-F]{4})[\s:]+?((?<databyte>[0-9a-fA-F]{2})\s*){1,8}$";
        private readonly string _fileName;
        public TextCanLogSource(string FileName) { _fileName = FileName; }

        public IEnumerable<CanFrame> ReadFrames()
        {
            var regex = new Regex(Pattern, RegexOptions.Compiled);
            using (TextReader tr = File.OpenText(_fileName))
            {
                string line;
                while ((line = tr.ReadLine()) != null)
                {
                    Match match = regex.Match(line);
                    if (match.Success)
                    {
                        ushort desc = Convert.ToUInt16(match.Groups["descriptor"].Value, 16);
                        List<Capture> c = match.Groups["databyte"].Captures.OfType<Capture>().ToList();
                        var bs = c.Select(bc => Convert.ToByte(bc.Value, 16)).ToArray();
                        yield return CanFrame.NewWithDescriptor(desc, bs);
                    }
                    else
                        Debug.WriteLine("{0}  XXX", line);
                }
            }
        }
    }
}
