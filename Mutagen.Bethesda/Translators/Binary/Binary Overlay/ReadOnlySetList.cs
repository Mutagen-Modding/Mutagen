using Noggog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public class ReadOnlySetList<T> : List<T>, IReadOnlySetList<T>
    {
        public bool HasBeenSet => true;
    }
}
