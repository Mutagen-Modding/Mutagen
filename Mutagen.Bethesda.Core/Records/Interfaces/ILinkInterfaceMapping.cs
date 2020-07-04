using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Core
{
    public interface ILinkInterfaceMapping
    {
        GameRelease GameRelease { get; }
        IReadOnlyDictionary<Type, Type[]> InterfaceToObjectTypes { get; } 
    }
}
