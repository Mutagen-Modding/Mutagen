using Loqui;
using Mutagen.Bethesda.Binary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mutagen.Bethesda.Internals;

namespace Mutagen.Bethesda
{
    public partial class Group<T>
        where T : ILoquiObject<T>, IFormID
    {
        static partial void FillBinary_ContainedRecordType_Custom<T_ErrMask>(
            MutagenFrame frame, 
            Group<T> item,
            int fieldIndex,
            Func<Group_ErrorMask<T_ErrMask>> errorMask)
            where T_ErrMask : class, IErrorMask<T_ErrMask>, new()
        {
            frame.Reader.Position += 4;
        }

        static partial void WriteBinary_ContainedRecordType_Custom<T_ErrMask>(
            MutagenWriter writer, 
            Group<T> item,
            int fieldIndex, 
            Func<Group_ErrorMask<T_ErrMask>> errorMask)
            where T_ErrMask : class, IErrorMask<T_ErrMask>, new()
        {
            Mutagen.Bethesda.Binary.StringBinaryTranslation.Instance.Write(
                writer,
                GRUP_RECORD_TYPE.Type,
                doMasks: errorMask != null,
                nullTerminate: false,
                errorMask: out var err);
            ErrorMask.HandleErrorMask(
                errorMask,
                fieldIndex,
                err);
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
