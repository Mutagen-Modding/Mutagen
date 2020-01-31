using Loqui.Internal;
using Mutagen.Bethesda.Internals;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public interface IBinaryWriteTranslator
    {
        void Write(
            MutagenWriter writer,
            object item,
            MasterReferences masterReferences,
            RecordTypeConverter recordTypeConverter = null);
    }

    public interface IBinaryItem
    {
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        object BinaryWriteTranslator { get; }
        void WriteToBinary(
            MutagenWriter writer,
            MasterReferences masterReferences,
            RecordTypeConverter recordTypeConverter = null);
    }
}
