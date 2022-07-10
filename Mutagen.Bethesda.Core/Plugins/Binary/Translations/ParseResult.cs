namespace Mutagen.Bethesda.Plugins.Binary.Translations;

internal readonly struct ParseResult
{
    public readonly bool KeepParsing;
    public readonly int? ParsedIndex;
    public readonly RecordType? DuplicateParseMarker;
    public readonly int? LengthOverride;

    public ParseResult(
        int? parsedIndex,
        bool keepParsing,
        RecordType? dupParse,
        int? lengthOverride)
    {
        KeepParsing = keepParsing;
        ParsedIndex = parsedIndex;
        DuplicateParseMarker = dupParse;
        LengthOverride = lengthOverride;
    }

    public ParseResult(
        int? parsedIndex,
        RecordType? dupParse)
    {
        KeepParsing = true;
        ParsedIndex = parsedIndex;
        DuplicateParseMarker = dupParse;
        LengthOverride = null;
    }

    public static implicit operator ParseResult(int? lastParsed)
    {
        return new ParseResult(
            parsedIndex: lastParsed,
            keepParsing: true,
            dupParse: null,
            lengthOverride: null);
    }

    public static implicit operator ParseResult(int lastParsed)
    {
        return new ParseResult(
            parsedIndex: lastParsed,
            keepParsing: true,
            dupParse: null,
            lengthOverride: null);
    }

    public static implicit operator ParseResult(PreviousParse lastParsed)
    {
        return new ParseResult(
            parsedIndex: lastParsed.ParsedIndex,
            keepParsing: true,
            dupParse: null,
            lengthOverride: null);
    }

    public static ParseResult Stop => new ParseResult();

    public static ParseResult OverrideLength(uint length) => new ParseResult(
        parsedIndex: null,
        keepParsing: true,
        dupParse: null,
        lengthOverride: checked((int)length));
}