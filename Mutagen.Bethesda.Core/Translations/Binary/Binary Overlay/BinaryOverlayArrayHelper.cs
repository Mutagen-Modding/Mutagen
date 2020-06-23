using Mutagen.Bethesda.Internals;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public static class BinaryOverlayArrayHelper
    {
        public static ReadOnlyMemorySlice<T> EnumSliceFromFixedSize<T>(
            ReadOnlyMemorySlice<byte> mem,
            int amount,
            byte enumLength)
            where T : struct, Enum
        {
            T[] ret = new T[amount];
            for (int i = 0; i < amount; i++)
            {
                ret[i] = EnumExt.Parse<T>(BinaryPrimitives.ReadInt32LittleEndian(mem.Slice(i * enumLength)), default(T));
            }
            return ret;
        }

        public static ReadOnlyMemorySlice<T> LoquiSliceFromFixedSize<T>(
            ReadOnlyMemorySlice<byte> mem,
            int amount,
            int length,
            BinaryOverlayFactoryPackage package,
            RecordTypeConverter? recordTypeConverter,
            BinaryOverlay.ConverterFactory<T> getter)
        {
            T[] ret = new T[amount];
            for (int i = 0; i < amount; i++)
            {
                ret[i] = getter(new OverlayStream(mem.Slice(i * length), package), package, recordTypeConverter);
            }
            return ret;
        }
    }
}
