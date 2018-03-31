using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog.Notifying;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class OblivionMod : IMod, ILinkContainer
    {
        public INotifyingListGetter<MasterReference> MasterReferences => this.TES4.MasterReferences;

        public static IReadOnlyCollection<RecordType> NonTypeGroups { get; } = new HashSet<RecordType>(
            new RecordType[]
            {
                new RecordType("CELL"),
                new RecordType("WRLD"),
                new RecordType("DIAL"),
            });

        //static partial void FillBinary_Cells_Custom(MutagenFrame frame, OblivionMod item, int fieldIndex, Func<OblivionMod_ErrorMask> errorMask)
        //{
        //    frame.Position += 16;
        //    item.Cells.LastModified = frame.Reader.ReadBytes(4);
        //    while (!frame.Complete)
        //    {
        //        var cellBlockRec = HeaderTranslation.ReadNextRecordType(frame.Reader, out var blockLen);
        //        if (!cellBlockRec.Equals("GRUP"))
        //        {
        //            throw new ArgumentException();
        //        }
        //        frame.Position -= 8;
        //        using (var cellBlockFrame = frame.Spawn(blockLen))
        //        {
        //            CellBlock block = new CellBlock();
        //            item.Cells.Blocks.Add(block);
        //            frame.Position += 12;
        //            block.GroupType = (GroupTypeEnum)cellBlockFrame.Reader.ReadInt32();
        //            block.LastModified = cellBlockFrame.Reader.ReadBytes(4);
        //            while (!cellBlockFrame.Complete)
        //            {
        //                var cellSubBlockRec = HeaderTranslation.ReadNextRecordType(cellBlockFrame.Reader, out var subBlockLen);
        //                if (!cellBlockRec.Equals("GRUP"))
        //                {
        //                    throw new ArgumentException();
        //                }
        //                frame.Position -= 8;
        //                using (var cellSubBlockFrame = cellBlockFrame.Spawn(subBlockLen))
        //                {
        //                    CellSubBlock subBlock = new CellSubBlock();
        //                    block.SubBlocks.Add(subBlock);
        //                    frame.Position += 12;
        //                    block.GroupType = (GroupTypeEnum)cellBlockFrame.Reader.ReadInt32();
        //                    block.LastModified = cellBlockFrame.Reader.ReadBytes(4);
        //                    while (!cellSubBlockFrame.Complete)
        //                    {
        //                        subBlock.Cells.Add(
        //                            Cell.Create_Binary(cellSubBlockFrame));
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        //static partial void WriteBinary_Cells_Custom(MutagenWriter writer, OblivionMod item, int fieldIndex, Func<OblivionMod_ErrorMask> errorMask)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
