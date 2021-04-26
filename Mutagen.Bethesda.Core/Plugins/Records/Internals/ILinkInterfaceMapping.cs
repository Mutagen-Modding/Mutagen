using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Plugins.Records.Internals
{
    public interface ILinkInterfaceMapping
    {
        GameCategory GameCategory { get; }
        IReadOnlyDictionary<Type, Type[]> InterfaceToObjectTypes { get; } 
    }
}
