using System;
using System.Collections.Generic;

namespace Mutagen.Bethesda.Plugins.Records.Mapping;

internal interface IAspectInterfaceMapping
{
    GameCategory GameCategory { get; }
    IReadOnlyDictionary<Type, InterfaceMappingResult> InterfaceToObjectTypes { get; }
}