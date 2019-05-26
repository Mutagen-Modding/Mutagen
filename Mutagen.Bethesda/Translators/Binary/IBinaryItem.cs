using Loqui.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public interface IBinaryTranslator
    {
        void Write(
            MutagenWriter writer,
            object item,
            MasterReferences masterReferences,
            RecordTypeConverter recordTypeConverter = null,
            ErrorMaskBuilder errorMask = null);
    }

    public interface IBinaryItem
    {
        IBinaryTranslator BinaryTranslator { get; }
    }
}
