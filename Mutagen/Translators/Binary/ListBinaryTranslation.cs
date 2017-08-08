using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noggog;
using Loqui;
using Noggog.Notifying;
using System.IO;

namespace Mutagen.Binary
{
    public class ListBinaryTranslation<T, M> : ContainerBinaryTranslation<T, M>
    {
        public static readonly ListBinaryTranslation<T, M> Instance = new ListBinaryTranslation<T, M>();

        public override void WriteSingleItem<ErrMask>(BinaryWriter writer, BinarySubWriteDelegate<T, ErrMask> transl, T item, bool doMasks, out ErrMask maskObj)
        {
            transl(item, doMasks, out maskObj);
        }

        public override TryGet<T> ParseSingleItem(BinaryReader reader, BinarySubParseDelegate<T, M> transl, bool doMasks, out M maskObj)
        {
            return transl(reader, doMasks, out maskObj);
        }
    }
}
