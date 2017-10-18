using Noggog;
using System;

namespace Mutagen.Binary
{
    public delegate TryGet<T> BinarySubParseDelegate<T, M>(MutagenReader reader, bool doMasks, out M maskObj);
    public delegate void BinarySubWriteDelegate<in T, M>(T item, bool doMasks, out M maskObj);

    public interface IBinaryTranslation<T, M>
    {
        void Write(MutagenWriter writer, T item, ContentLength length, bool doMasks, out M maskObj);
        TryGet<T> Parse(MutagenReader reader, ContentLength length, bool doMasks, out M maskObj);
    }
}
