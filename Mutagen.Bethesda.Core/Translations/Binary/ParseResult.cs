using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public struct ParseResult
    {
        public bool KeepParsing;
        public int? ParsedIndex;

        public ParseResult(
            int? parsedIndex,
            bool keepParsing)
        {
            KeepParsing = keepParsing;
            ParsedIndex = parsedIndex;
        }

        public static implicit operator ParseResult(int? lastParsed)
        {
            return new ParseResult(
                parsedIndex: lastParsed,
                keepParsing: true);
        }

        public static implicit operator ParseResult(int lastParsed)
        {
            return new ParseResult(
                parsedIndex: lastParsed,
                keepParsing: true);
        }

        public static ParseResult Stop => new ParseResult();
    }
}
