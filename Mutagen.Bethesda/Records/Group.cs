using Loqui;
using Mutagen.Bethesda.Binary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mutagen.Bethesda.Internals;
using Noggog;
using Loqui.Internal;

namespace Mutagen.Bethesda
{
    public partial class Group<T>
        where T : ILoquiObject<T>, IFormID
    {
        static partial void FillBinary_ContainedRecordType_Custom(
            MutagenFrame frame,
            Group<T> item,
            ErrorMaskBuilder errorMask)
        {
            frame.Reader.Position += 4;
        }

        static partial void WriteBinary_ContainedRecordType_Custom(
            MutagenWriter writer,
            Group<T> item,
            ErrorMaskBuilder errorMask)
        {
            Mutagen.Bethesda.Binary.Int32BinaryTranslation.Instance.Write(
                writer,
                GRUP_RECORD_TYPE.TypeInt,
                errorMask: errorMask);
        }
    }

    public enum GroupTypeEnum
    {
        Type = 0,
        WorldChildren = 1,
        InteriorCellBlock = 2,
        InteriorCellSubBlock = 3,
        ExteriorCellBlock = 4,
        ExteriorCellSubBlock = 5,
        CellChildren = 6,
        TopicChildren = 7,
        CellPersistentChildren = 8,
        CellTemporaryChildren = 9,
        CellVisibleDistantChildren = 10,
    }

    public static class GroupExt
    {
        public static async Task Write_XmlFolder<T, T_ErrMask>(
            this Group<T> group,
            DirectoryPath dir,
            Func<IErrorMask> errMaskFunc,
            int index,
            bool doMasks)
            where T : MajorRecord, ILoquiObject<T>, IFormID
            where T_ErrMask : MajorRecord_ErrorMask, IErrorMask<T_ErrMask>, new()
        {
            Group_ErrorMask<T_ErrMask> grupErrMask = null;
            try
            {
                dir.Create();
                List<Task<MajorRecord_ErrorMask>> writeTasks = new List<Task<MajorRecord_ErrorMask>>();
                int counter = 0;
                foreach (var item in group.Items.Values)
                {
                    writeTasks.Add(Task.Run(() =>
                    {
                        item.Write_XML_Folder(
                            path: Path.Combine(dir.Path, $"{counter++} - {item.FormID.IDString()} - {item.EditorID}.xml"),
                            errorMask: out var itemErrMask,
                            doMasks: doMasks);
                        return itemErrMask;
                    }));
                }
                var errMasks = await Task.WhenAll(writeTasks);
                foreach (var itemErrMask in errMasks)
                {
                    if (itemErrMask == null) continue;
                    if (grupErrMask == null)
                    {
                        grupErrMask = new Group_ErrorMask<T_ErrMask>();
                        errMaskFunc().SetNthMask(
                            index,
                            new MaskItem<Exception, Group_ErrorMask<T_ErrMask>>(null, grupErrMask));
                    }
                    ErrorMask.HandleErrorMaskAddition<IErrorMask, T_ErrMask>(
                        mask: grupErrMask,
                        index: (int)Group_FieldIndex.Items,
                        errMaskObj: (T_ErrMask)itemErrMask);
                }
            }
            catch (Exception ex)
            when (doMasks)
            {
                errMaskFunc().Overall = ex;
            }
        }
    }
}
