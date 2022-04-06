using System;
using System.Collections.Generic;

namespace Mutagen.Bethesda.Plugins.Records.Mapping;

internal interface ILinkInterfaceMapping
{
    GameCategory GameCategory { get; }
    IReadOnlyDictionary<Type, InterfaceMappingResult> InterfaceToObjectTypes { get; } 
}