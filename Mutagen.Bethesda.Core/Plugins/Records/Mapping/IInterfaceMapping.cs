using System;
using System.Collections.Generic;

namespace Mutagen.Bethesda.Plugins.Records.Mapping;

internal interface IInterfaceMapping
{
    GameCategory GameCategory { get; }
    IReadOnlyDictionary<Type, InterfaceMappingResult> InterfaceToObjectTypes { get; } 
}