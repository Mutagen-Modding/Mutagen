using System;
using System.Collections.Generic;
using Loqui;
using Mutagen.Bethesda.Plugins.Cache.Internals;
using Mutagen.Bethesda.Plugins.Records.Mapping;
using Noggog;

namespace Mutagen.Bethesda.Plugins;

/// <summary>
/// A static class to house initialization warmup logic
/// </summary>
public static class Warmup
{
    /// <summary>
    /// Will initialize internal registrations.<br/>
    /// Not required to call, but can be used to warm up ahead of time.
    /// </summary>
    public static void Init()
    {
        List<IProtocolRegistration> protocols = new()
        {
            new ProtocolDefinition_Bethesda()
        };
        foreach (var category in EnumExt<GameCategory>.Values)
        {
            var obj = Activator.CreateInstance(
                $"Mutagen.Bethesda.{category}",
                $"Loqui.ProtocolDefinition_{category}");
            var regis = obj?.Unwrap() as IProtocolRegistration;
            if (regis == null) continue;
            protocols.Add(regis);
        }
        Loqui.Initialization.SpinUp(protocols.ToArray());
        MetaInterfaceMapping.Warmup();
        OverrideMaskRegistrations.Warmup();
    }
}