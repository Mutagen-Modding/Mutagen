using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Oblivion.Internals;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class Armor
    {
        static partial void FillBinary_ArmorValue_Custom(MutagenFrame frame, Armor item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
        {
            if (UInt16BinaryTranslation.Instance.Parse(
                frame.Spawn(snapToFinalPosition: false),
                out var val,
                errorMask))
            {
                item.ArmorValue = val / 100f;
            }
        }

        static partial void WriteBinary_ArmorValue_Custom(MutagenWriter writer, Armor item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
        {
            UInt16BinaryTranslation.Instance.Write(
                writer,
                (ushort)(item.ArmorValue * 100),
                errorMask);
        }
    }
}
