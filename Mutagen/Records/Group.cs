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
        private static void FillBinaryContainedRecordType(BinaryReader reader, Group<T> group)
        {
            reader.BaseStream.Position += 4;
        }
    }
}
