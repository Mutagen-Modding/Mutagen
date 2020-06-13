using Mutagen.Bethesda.Binary;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Oblivion.Internals
{
    public partial class ArmorDataBinaryCreateTranslation
    {
        public static float GetArmorValue(ushort i) => i / 100f;

        static partial void FillBinaryArmorValueCustom(MutagenFrame frame, IArmorData item)
        {
            if (UInt16BinaryTranslation.Instance.Parse(
                frame,
                out var val))
            {
                item.ArmorValue = GetArmorValue(val);
            }
        }
    }

    public partial class ArmorDataBinaryWriteTranslation
    {
        static partial void WriteBinaryArmorValueCustom(MutagenWriter writer, IArmorDataGetter item)
        {
            UInt16BinaryTranslation.Instance.Write(
                writer,
                (ushort)(item.ArmorValue * 100));
        }
    }

    public partial class ArmorDataBinaryOverlay
    {
        public bool GetArmorValueIsSetCustom() => true;
        public float GetArmorValueCustom(int location)
        {
            return ArmorDataBinaryCreateTranslation.GetArmorValue(BinaryPrimitives.ReadUInt16LittleEndian(_data));
        }
    }
}
