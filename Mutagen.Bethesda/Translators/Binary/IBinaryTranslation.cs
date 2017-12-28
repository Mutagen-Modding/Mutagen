using Noggog;
using System;

namespace Mutagen.Bethesda.Binary
{
    public delegate TryGet<T> BinarySubParseDelegate<T, M>(MutagenFrame reader, bool doMasks, out M maskObj);
    public delegate TryGet<T> BinarySubParseRecordDelegate<T, M>(MutagenFrame reader, RecordType header, bool doMasks, out M maskObj);
    public delegate void BinarySubWriteDelegate<in T, M>(T item, bool doMasks, out M maskObj);

    public interface IBinaryTranslation<T, M>
    {
        void Write(MutagenWriter writer, T item, ContentLength length, bool doMasks, out M maskObj);
        TryGet<T> Parse(MutagenFrame reader, bool doMasks, out M maskObj);
    }
}
