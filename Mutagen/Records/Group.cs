using Loqui;
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
        private static void FillBinary_ContainedRecordType(BinaryReader reader, Group<T> item, bool doMasks, out Exception errorMask)
        {
            reader.BaseStream.Position += 4;
            errorMask = null;
        }

        internal static void WriteBinary_ContainedRecordType(BinaryWriter writer, IGroupGetter<T> item, bool doMasks, out Exception errorMask)
        {
            Mutagen.Binary.StringBinaryTranslation.Instance.Write(
                writer,
                T_RecordType.Type,
                doMasks,
                out errorMask);
        }
    }
}
