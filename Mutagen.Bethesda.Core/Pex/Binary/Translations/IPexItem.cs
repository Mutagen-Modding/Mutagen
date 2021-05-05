using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mutagen.Bethesda.Pex.Binary.Translations
{
    public interface IPexWriteTranslator
    {
        void Write(
            PexWriter write,
            object item);
    }

    public interface IPexItem
    {
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        object PexWriteTranslator { get; }
        void WriteToBinary(PexWriter write);
    }
}
