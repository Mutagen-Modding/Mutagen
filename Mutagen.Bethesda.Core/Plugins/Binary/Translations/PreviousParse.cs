namespace Mutagen.Bethesda.Plugins.Binary.Translations
{
    public struct PreviousParse
    {
        public readonly int? ParsedIndex;

        public PreviousParse(int? parsedIndex)
        {
            ParsedIndex = parsedIndex;
        }

        public static implicit operator PreviousParse(ParseResult lastParsed)
        {
            return new PreviousParse(
                parsedIndex: lastParsed.ParsedIndex);
        }
    }
}