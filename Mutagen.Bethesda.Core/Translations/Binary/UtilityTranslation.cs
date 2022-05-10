using Noggog;
using System.Diagnostics.CodeAnalysis;

namespace Mutagen.Bethesda.Translations.Binary;

public static class UtilityTranslation
{
    private static readonly byte[] _Zeros = new byte[8];
    public static ReadOnlyMemorySlice<byte> Zeros => new(_Zeros);

    public delegate bool BinarySubParseDelegate<TWriter, TItem>(
        TWriter reader,
        [MaybeNullWhen(false)] out TItem item);
    public delegate void BinarySubWriteDelegate<TWriter, TItem>(
        TWriter writer,
        TItem item);
}