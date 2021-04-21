using Mutagen.Bethesda.Records.Binary.Streams;
using System;
using System.Collections.Generic;
using static Mutagen.Bethesda.Translations.Binary.UtilityTranslation;

namespace Mutagen.Bethesda.Records.Binary.Translations
{
    public class DictBinaryTranslation<TValue>
    {
        public static readonly DictBinaryTranslation<TValue> Instance = new();

        public IReadOnlyDictionary<TEnum, TValue> Parse<TEnum>(
            MutagenFrame reader,
            IDictionary<TEnum, TValue> item,
            BinarySubParseDelegate<MutagenFrame, TValue> transl)
            where TEnum : struct, Enum, IConvertible
        {
            foreach (var e in EnumBinaryTranslation<TEnum>.Values)
            {
                if (!transl(reader, out var parse))
                {
                    throw new ArgumentException();
                }
                item[e] = parse;
            }
            return (IReadOnlyDictionary<TEnum, TValue>)item;
        }

        public void Write<TEnum>(
            MutagenWriter writer,
            IReadOnlyDictionary<TEnum, TValue> items,
            BinarySubWriteDelegate<MutagenWriter, TValue> transl)
            where TEnum : struct, Enum, IConvertible
        {
            foreach (var e in EnumBinaryTranslation<TEnum>.Values)
            {
                if (items.TryGetValue(e, out var val))
                {
                    transl(writer, val);
                }
                else
                {
                    transl(writer, default!);
                }
            }
        }
    }
}
