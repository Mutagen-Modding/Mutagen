using Loqui;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public partial class ListGroup<T>
        where T : ILoquiObject<T>
    {
        static partial void FillBinary_ContainedRecordType_Custom<T_ErrMask>(
            MutagenFrame frame, 
            ListGroup<T> item, 
            int fieldIndex,
            Func<ListGroup_ErrorMask<T_ErrMask>> errorMask) 
            where T_ErrMask : class, IErrorMask<T_ErrMask>, new()
        {
            frame.Reader.Position += 4;
        }

        static partial void WriteBinary_ContainedRecordType_Custom<T_ErrMask>(
            MutagenWriter writer,
            ListGroup<T> item, 
            int fieldIndex,
            Func<ListGroup_ErrorMask<T_ErrMask>> errorMask) 
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
}
