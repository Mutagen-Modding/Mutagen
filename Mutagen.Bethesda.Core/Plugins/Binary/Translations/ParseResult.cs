using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Plugins.Binary.Translations
{
    public struct ParseResult
    {
        public readonly bool KeepParsing;
        public readonly int? ParsedIndex;
        public readonly RecordType? DuplicateParseMarker;

        public ParseResult(
            int? parsedIndex,
            bool keepParsing,
            RecordType? dupParse)
        {
            KeepParsing = keepParsing;
            ParsedIndex = parsedIndex;
            DuplicateParseMarker = dupParse;
        }

        public ParseResult(
            int? parsedIndex,
            RecordType? dupParse)
        {
            KeepParsing = true;
            ParsedIndex = parsedIndex;
            DuplicateParseMarker = dupParse;
        }

        public static implicit operator ParseResult(int? lastParsed)
        {
            return new ParseResult(
                parsedIndex: lastParsed,
                keepParsing: true,
                dupParse: null);
        }

        public static implicit operator ParseResult(int lastParsed)
        {
            return new ParseResult(
                parsedIndex: lastParsed,
                keepParsing: true,
                dupParse: null);
        }

        public static implicit operator ParseResult(PreviousParse lastParsed)
        {
            return new ParseResult(
                parsedIndex: lastParsed.ParsedIndex,
                keepParsing: true,
                dupParse: null);
        }

        public static ParseResult Stop => new ParseResult();
    }
}
