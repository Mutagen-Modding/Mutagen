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

namespace Mutagen.Bethesda
{
    public partial class Group<T>
        where T : MajorRecord, ILoquiObjectGetter
    {
        static partial void FillBinary_ContainedRecordType_Custom<T_ErrMask>(
            MutagenFrame frame,
            Group<T> item,
            int fieldIndex,
            Func<Group_ErrorMask<T_ErrMask>> errorMask)
            where T_ErrMask : MajorRecord_ErrorMask, IErrorMask<T_ErrMask>, new()
        {
            frame.Reader.Position += 4;
        }

        static partial void WriteBinary_ContainedRecordType_Custom<T_ErrMask>(
            MutagenWriter writer,
            Group<T> item,
            int fieldIndex,
            Func<Group_ErrorMask<T_ErrMask>> errorMask)
            where T_ErrMask : MajorRecord_ErrorMask, IErrorMask<T_ErrMask>, new()
        {
            Mutagen.Bethesda.Binary.StringBinaryTranslation.Instance.Write(
                writer,
                T_RecordType.Type,
                doMasks: errorMask != null,
                nullTerminate: false,
                errorMask: out var err);
            ErrorMask.HandleErrorMask(
                errorMask,
                fieldIndex,
                err);
        }

        public async Task Write_XmlFolder<T_ErrMask>(
            DirectoryPath dir,
            Func<IErrorMask> errMaskFunc,
            int index,
            bool doMasks)
            where T_ErrMask : MajorRecord_ErrorMask, IErrorMask<T_ErrMask>, new()
        {
            Group_ErrorMask<T_ErrMask> grupErrMask = null;
            try
            {
                dir.Create();
                List<Task<MajorRecord_ErrorMask>> writeTasks = new List<Task<MajorRecord_ErrorMask>>();
                int counter = 0;
                foreach (var item in this.Items.Values)
                {
                    writeTasks.Add(Task.Run(() =>
                    {
                        item.Write_XML(
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
                        creator: () => grupErrMask,
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
}
