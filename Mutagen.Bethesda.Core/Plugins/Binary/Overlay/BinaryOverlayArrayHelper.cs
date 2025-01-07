using Noggog;
using System.Buffers.Binary;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Plugins.Binary.Overlay;

internal static class BinaryOverlayArrayHelper
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
            ret[i] = Enums<T>.TryConvert(BinaryPrimitives.ReadInt32LittleEndian(mem.Slice(i * enumLength)), default(T));
        }
        return ret;
    }

    public static ReadOnlyMemorySlice<float> FloatSliceFromFixedSize(
        ReadOnlyMemorySlice<byte> mem,
        int amount)
    {
        return mem.Span.Slice(0, amount * 4).AsFloatSpan().ToArray();
    }

    public static ReadOnlyMemorySlice<IFormLinkGetter<TMajorGetter>> FormLinkSliceFromFixedSize<TMajorGetter>(
        ReadOnlyMemorySlice<byte> mem,
        int amount,
        IReadOnlySeparatedMasterPackage masterReferences)
        where TMajorGetter : class, IMajorRecordGetter
    {
        var intSpan = mem.Span.Slice(0, amount * 4).AsUInt32Span();
        var ret = new IFormLinkGetter<TMajorGetter>[intSpan.Length];
        for (int i = 0; i < intSpan.Length; i++)
        {
            var raw = intSpan[i];
            ret[i] = new FormLink<TMajorGetter>(FormKey.Factory(masterReferences, new FormID(raw), reference: true));
        }
        return ret;
    }
}