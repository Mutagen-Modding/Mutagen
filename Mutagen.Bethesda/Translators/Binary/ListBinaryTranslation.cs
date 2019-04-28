using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noggog;
using Loqui;
using Noggog.Notifying;
using System.IO;
using Loqui.Internal;

namespace Mutagen.Bethesda.Binary
{
    public class ListBinaryTranslation<T> : ContainerBinaryTranslation<T>
    {
        public static readonly ListBinaryTranslation<T> Instance = new ListBinaryTranslation<T>();
    }
}
