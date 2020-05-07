using Noggog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public class DictBinaryTranslation<TValue>
    {
        public static readonly DictBinaryTranslation<TValue> Instance = new DictBinaryTranslation<TValue>();

        public IReadOnlyDictionary<TEnum, TValue> Parse<TEnum>(
            MutagenFrame frame,
            IDictionary<TEnum, TValue> item,
            UtilityTranslation.BinarySubParseDelegate<TValue> transl)
            where TEnum : struct, Enum, IConvertible
        {
            for (int i = 0; i < EnumBinaryTranslation<TEnum>.EnumSize; i++)
            {
                if (!transl(frame, out var parse))
                {
                    throw new ArgumentException();
                }
                item[EnumExt.GetNth<TEnum>(i)] = parse;
            }
            return (IReadOnlyDictionary<TEnum, TValue>)item;
        }

        public void Write<TEnum>(
            MutagenWriter writer,
            IReadOnlyDictionary<TEnum, TValue> items,
            UtilityTranslation.BinarySubWriteDelegate<TValue> transl)
            where TEnum : struct, Enum, IConvertible
        {
            for (int i = 0; i < EnumBinaryTranslation<TEnum>.EnumSize; i++)
            {
                if (items.TryGetValue(EnumExt.GetNth<TEnum>(i), out var val))
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
