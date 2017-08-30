using Noggog;
using System;
using System.IO;

namespace Mutagen.Binary
{
    public delegate TryGet<T> BinarySubParseDelegate<T, M>(BinaryReader reader, bool doMasks, out M maskObj);
    public delegate void BinarySubWriteDelegate<in T, M>(T item, bool doMasks, out M maskObj);

    public interface IBinaryTranslation<T, M>
    {
        void Write(BinaryWriter writer, T item, bool doMasks, out M maskObj);
        TryGet<T> Parse(BinaryReader reader, long length, bool doMasks, out M maskObj);
    }
}
