using Mutagen.Bethesda.Plugins.Binary.Streams;

namespace Mutagen.Bethesda.Plugins.Binary.Translations
{
    public struct TypedParseParams
    {
        public readonly RecordTypeConverter? RecordTypeConverter;
        public readonly int? LengthOverride;

        public TypedParseParams(
            int? lengthOverride, 
            RecordTypeConverter? recordTypeConverter)
        {
            LengthOverride = lengthOverride;
            RecordTypeConverter = recordTypeConverter;
        }

        public static implicit operator TypedParseParams(RecordTypeConverter? converter)
        {
            return new TypedParseParams(lengthOverride: null, converter);
        }
    }
}