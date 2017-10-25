using Loqui;
using Mutagen.Binary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen
{
    public partial class Group<T>
        where T : MajorRecord, ILoquiObjectGetter
    {
        private static void FillBinary_ContainedRecordType<M>(
            MutagenFrame frame,
            Group<T> item,
            bool doMasks,
            int fieldIndex,
            Func<M> errorMask)
            where M : IErrorMask
        {
            frame.Reader.Position += 4;
        }

        internal static void WriteBinary_ContainedRecordType(MutagenWriter writer, IGroupGetter<T> item, bool doMasks, out Exception errorMask)
        {
            Mutagen.Binary.StringBinaryTranslation.Instance.Write(
                writer,
                T_RecordType.Type,
                doMasks: doMasks,
                nullTerminate: false,
                errorMask: out errorMask);
        }
    }
}
