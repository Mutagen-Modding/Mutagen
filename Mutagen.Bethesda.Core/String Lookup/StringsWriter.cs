using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// Class for compiling strings of various languages, and exporting them to a .strings file
    /// </summary>
    public class StringsWriter
    {
        private readonly List<KeyValuePair<Language, string>[]> _strings = new List<KeyValuePair<Language, string>[]>();

        public uint Register(ITranslatedStringGetter str)
        {
            lock (_strings)
            {
                _strings.Add(str.ToArray());
                try
                {
                    return checked((uint)_strings.Count);
                }
                catch (OverflowException)
                {
                    throw new OverflowException("Too many translated strings for current system to handle.");
                }
            }
        }
    }
}
