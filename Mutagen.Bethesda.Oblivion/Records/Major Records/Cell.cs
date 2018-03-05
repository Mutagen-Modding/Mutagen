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
    public partial class Cell
    {
        [Flags]
        public enum Flag
        {
            IsInteriorCell = 0x0001,
            HasWater = 0x0002,
            InvertFastTravelBehavior = 0x0004,
            ForceHideLand = 0x0008,
            PublicPlace = 0x0020,
            HandChanged = 0x0040,
            BehaveLikeExteriod = 0x0080,
        }

        static partial void FillBinary_CellContents_Custom(MutagenFrame frame, Cell item, Func<Cell_ErrorMask> errorMask)
        {
            if ("GRUP".Equals(HeaderTranslation.ReadNextRecordType(frame.Reader, out var persistentLength).Type))
            {
                throw new ArgumentException();
            }            var persistentTryGet = Mutagen.Bethesda.Binary.ListBinaryTranslation<Placed, MaskItem<Exception, Placed_ErrorMask>>.Instance.ParseRepeatedItem(
                frame: frame,
                triggeringRecord: Placed_Registration.TriggeringRecordTypes,
                fieldIndex: (int)Cell_FieldIndex.Persistent,
                lengthLength: new ContentLength(4),
                errorMask: errorMask,
                transl: (MutagenFrame r, bool listDoMasks, out MaskItem<Exception, Placed_ErrorMask> listSubMask) =>
                {
                    return LoquiBinaryTranslation<Placed, Placed_ErrorMask>.Instance.Parse(
                        frame: r,
                        doMasks: listDoMasks,
                        errorMask: out listSubMask);
                }
                );
            item.Persistent.SetIfSucceeded(persistentTryGet);

        }

        static partial void WriteBinary_CellContents_Custom(MutagenWriter writer, Cell item, Func<Cell_ErrorMask> errorMask)
        {
            throw new NotImplementedException();
        }
    }
}
