using System;
using System.Collections.Generic;
using System.Text;
using Loqui;

namespace Mutagen.Bethesda.Plugins.Records.Internals
{
    public interface ILinkInterfaceMapping
    {
        GameCategory GameCategory { get; }
        IReadOnlyDictionary<Type, ILoquiRegistration[]> InterfaceToObjectTypes { get; } 
    }
}
