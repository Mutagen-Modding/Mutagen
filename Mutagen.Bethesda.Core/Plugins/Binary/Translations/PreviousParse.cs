namespace Mutagen.Bethesda.Plugins.Binary.Translations;

internal struct PreviousParse
{
    public readonly int? ParsedIndex;
    public readonly int? LengthOverride;

    public PreviousParse(int? parsedIndex, int? lengthOverride)
    {
        LengthOverride = lengthOverride;
        ParsedIndex = parsedIndex;
    }

    public static implicit operator PreviousParse(ParseResult lastParsed)
    {
        return new PreviousParse(
            parsedIndex: lastParsed.ParsedIndex,
            lengthOverride: lastParsed.LengthOverride);
    }
}