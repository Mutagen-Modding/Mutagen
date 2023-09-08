using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Translations.Binary;
using System.Buffers.Binary;
using Mutagen.Bethesda.Plugins.Binary.Translations;

namespace Mutagen.Bethesda.Oblivion;

partial class ArmorDataBinaryCreateTranslation
{
    public static float GetArmorValue(ushort i) => i / 100f;

    public static partial void FillBinaryArmorValueCustom(MutagenFrame frame, IArmorData item)
    {
        if (UInt16BinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Parse(
                frame,
                out var val))
        {
            item.ArmorValue = GetArmorValue(val);
        }
    }
}

partial class ArmorDataBinaryWriteTranslation
{
    public static partial void WriteBinaryArmorValueCustom(MutagenWriter writer, IArmorDataGetter item)
    {
        UInt16BinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Write(
            writer,
            (ushort)(item.ArmorValue * 100));
    }
}

partial class ArmorDataBinaryOverlay
{
    public bool GetArmorValueIsSetCustom() => true;
    public partial float GetArmorValueCustom(int location)
    {
        return ArmorDataBinaryCreateTranslation.GetArmorValue(BinaryPrimitives.ReadUInt16LittleEndian(_structData));
    }
}