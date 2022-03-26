using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Translations.Binary;
using System.Buffers.Binary;

namespace Mutagen.Bethesda.Oblivion.Internals
{
    public partial class ArmorDataBinaryCreateTranslation
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

    public partial class ArmorDataBinaryWriteTranslation
    {
        public static partial void WriteBinaryArmorValueCustom(MutagenWriter writer, IArmorDataGetter item)
        {
            UInt16BinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Write(
                writer,
                (ushort)(item.ArmorValue * 100));
        }
    }

    public partial class ArmorDataBinaryOverlay
    {
        public bool GetArmorValueIsSetCustom() => true;
        public partial float GetArmorValueCustom(int location)
        {
            return ArmorDataBinaryCreateTranslation.GetArmorValue(BinaryPrimitives.ReadUInt16LittleEndian(_data));
        }
    }
}
