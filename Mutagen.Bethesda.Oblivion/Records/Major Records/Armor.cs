using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Oblivion.Internals;

namespace Mutagen.Bethesda.Oblivion.Internals
{
    public partial class ArmorBinaryCreateTranslation
    {
        public static float GetArmorValue(ushort i) => i / 100f;

        static partial void FillBinaryArmorValueCustom(MutagenFrame frame, Armor item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
        {
            if (UInt16BinaryTranslation.Instance.Parse(
                frame,
                out var val))
            {
                item.ArmorValue = GetArmorValue(val);
            }
        }
    }

    public partial class ArmorBinaryWriteTranslation
    {
        static partial void WriteBinaryArmorValueCustom(MutagenWriter writer, IArmorInternalGetter item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
        {
            UInt16BinaryTranslation.Instance.Write(
                writer,
                (ushort)(item.ArmorValue * 100));
        }
    }

    public partial class ArmorBinaryWrapper
    {
        public float GetArmorValueCustom(ReadOnlySpan<byte> span)
        {
            return ArmorBinaryCreateTranslation.GetArmorValue(BinaryPrimitives.ReadUInt16LittleEndian(span));
        }
    }
}
