using Loqui;
using Loqui.Internal;
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
        static partial void FillBinary_ContainedRecordType_Custom(
            MutagenFrame frame, 
            ListGroup<T> item,
            ErrorMaskBuilder errorMask) 
        {
            frame.Reader.Position += 4;
        }

        static partial void WriteBinary_ContainedRecordType_Custom(
            MutagenWriter writer,
            ListGroup<T> item,
            ErrorMaskBuilder errorMask) 
        {
            Mutagen.Bethesda.Binary.Int32BinaryTranslation.Instance.Write(
                writer,
                GRUP_RECORD_TYPE.TypeInt,
                errorMask: errorMask);
        }
    }
}
