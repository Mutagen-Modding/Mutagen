using System;
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
    public partial class ArmorBinaryTranslation
    {
        static partial void FillBinaryArmorValueCustom(MutagenFrame frame, Armor item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
        {
            if (UInt16BinaryTranslation.Instance.Parse(
                frame,
                out var val))
            {
                item.ArmorValue = val / 100f;
            }
        }

        static partial void WriteBinaryArmorValueCustom(MutagenWriter writer, IArmorInternalGetter item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
        {
            UInt16BinaryTranslation.Instance.Write(
                writer,
                (ushort)(item.ArmorValue * 100));
        }
    }
}
