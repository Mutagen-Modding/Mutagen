namespace Mutagen.Bethesda.Plugins.Binary.Translations
{
    public struct PreviousSubrecordParse
    {
        public readonly int? ParsedIndex;

        public PreviousSubrecordParse(int? parsedIndex)
        {
            ParsedIndex = parsedIndex;
        }

        public static implicit operator PreviousSubrecordParse(ParseResult lastParsed)
        {
            return new PreviousSubrecordParse(
                parsedIndex: lastParsed.ParsedIndex);
        }
    }
}