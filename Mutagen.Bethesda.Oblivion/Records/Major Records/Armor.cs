using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Oblivion.Internals;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class Armor
    {
        static partial void FillBinary_ArmorValue_Custom(MutagenFrame frame, IArmor item, int fieldIndex, Func<Armor_ErrorMask> errorMask)
        {
            var tryGet = UInt16BinaryTranslation.Instance.Parse(
                frame,
                doMasks: errorMask != null,
                errorMask: out var mask);
            ErrorMask.HandleErrorMask(
                errorMask,
                fieldIndex,
                mask);
            if (tryGet.Succeeded)
            {
                item.ArmorValue = tryGet.Value / 100f;
            }
        }

        static partial void WriteBinary_ArmorValue_Custom(MutagenWriter writer, IArmorGetter item, int fieldIndex, Func<Armor_ErrorMask> errorMask)
        {
            UInt16BinaryTranslation.Instance.Write(
                writer,
                (ushort)(item.ArmorValue * 100),
                errorMask != null,
                out var mask);
            ErrorMask.HandleErrorMask(
                errorMask,
                fieldIndex,
                mask);
        }
    }
}
