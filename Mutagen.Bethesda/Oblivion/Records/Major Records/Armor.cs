using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Mutagen.Bethesda.Oblivion.Internals;

namespace Mutagen.Bethesda.Oblivion.Internals
{
    public partial class ArmorBinaryCreateTranslation
    {
        public static float GetArmorValue(ushort i) => i / 100f;

        static partial void FillBinaryArmorValueCustom(MutagenFrame frame, IArmorInternal item)
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
        static partial void WriteBinaryArmorValueCustom(MutagenWriter writer, IArmorGetter item)
        {
            UInt16BinaryTranslation.Instance.Write(
                writer,
                (ushort)(item.ArmorValue * 100));
        }
    }

    public partial class ArmorBinaryOverlay
    {
        public bool GetArmorValueIsSetCustom() => _DATALocation.HasValue;
        public float GetArmorValueCustom()
        {
            return ArmorBinaryCreateTranslation.GetArmorValue(BinaryPrimitives.ReadUInt16LittleEndian(_data.Span.Slice(_ArmorValueLocation)));
        }
    }
}
