using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda
{
    public interface IContextGetterMod<TMod> : IModGetter, IMajorRecordContextEnumerable<TMod>
        where TMod : IMod
    {
    }

    public interface IContextMod<TMod> : IMod, IContextGetterMod<TMod>
        where TMod : IContextMod<TMod>
    {
    }
}
