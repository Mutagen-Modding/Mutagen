using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Core
{
    public interface ILinkInterfaceMapping
    {
        GameCategory GameCategory { get; }
        IReadOnlyDictionary<Type, Type[]> InterfaceToObjectTypes { get; } 
    }
}
