using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Translations.Binary;
using static Mutagen.Bethesda.Translations.Binary.UtilityTranslation;

namespace Mutagen.Bethesda.Plugins.Binary.Translations;

public class DictBinaryTranslation<TValue>
{
    public static readonly DictBinaryTranslation<TValue> Instance = new();

    public IReadOnlyDictionary<TEnum, TValue> Parse<TEnum>(
        MutagenFrame reader,
        IDictionary<TEnum, TValue> item,
        BinarySubParseDelegate<MutagenFrame, TValue> transl)
        where TEnum : struct, Enum, IConvertible
    {
        foreach (var e in EnumBinaryTranslation<TEnum, MutagenFrame, MutagenWriter>.Values)
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
        foreach (var e in EnumBinaryTranslation<TEnum, MutagenFrame, MutagenWriter>.Values)
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
