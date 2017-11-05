using Loqui;
using Mutagen.Binary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mutagen.Internals;

namespace Mutagen
{
    public partial class Group<T>
        where T : MajorRecord, ILoquiObjectGetter
    {
        static partial void FillBinary_ContainedRecordType_Custom<T_ErrMask>(
            MutagenFrame frame, 
            IGroupGetter<T> item,
            bool doMasks, 
            int fieldIndex,
            Func<Group_ErrorMask<T_ErrMask>> errorMask) 
            where T_ErrMask : MajorRecord_ErrorMask, new()
        {
            frame.Reader.Position += 4;
        }

        static partial void WriteBinary_ContainedRecordType_Custom<T_ErrMask>(
            MutagenWriter writer, 
            IGroupGetter<T> item,
            bool doMasks, 
            int fieldIndex, 
            Func<Group_ErrorMask<T_ErrMask>> errorMask) 
            where T_ErrMask : MajorRecord_ErrorMask, new()
        {
            Mutagen.Binary.StringBinaryTranslation.Instance.Write(
                writer,
                T_RecordType.Type,
                doMasks: doMasks,
                nullTerminate: false,
                errorMask: out var err);
            ErrorMask.HandleErrorMask(
                errorMask,
                doMasks,
                fieldIndex,
                err);
        }
    }
}
