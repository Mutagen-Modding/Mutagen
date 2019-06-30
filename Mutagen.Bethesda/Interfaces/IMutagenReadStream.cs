using Noggog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public interface IMutagenReadStream : IBinaryReadStream
    {
        MetaDataConstants MetaData { get; }
        long OffsetReference { get; }
        IMutagenReadStream ReadAndReframe(int length);
        string ReadZString(int length);
    }
}
