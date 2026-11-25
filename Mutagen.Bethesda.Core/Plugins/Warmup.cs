using Loqui;
using Mutagen.Bethesda.Plugins.Cache.Internals;
using Mutagen.Bethesda.Plugins.Records.Mapping;

namespace Mutagen.Bethesda.Plugins;

/// <summary>
/// A static class to house initialization warmup logic
/// </summary>
public static class Warmup
{
    private static object _lock = new();
    private static bool _warmedUp = false;

    private static List<GameCategory> _registrations = new();

    /// <summary>
    /// Will initialize internal registrations.<br/>
    /// Not required to call, but can be used to warm up ahead of time.
    /// </summary>
    public static IReadOnlyList<GameCategory> Init()
    {
        lock (_lock)
        {
            if (_warmedUp) return _registrations;
            _warmedUp = true;
            
            List<IProtocolRegistration> protocols = new()
            {
                new ProtocolDefinition_Bethesda()
            };

            foreach (var module in GameRuntime.Modules)
            {
                protocols.Add(module.Protocol);
                _registrations.Add(module.Category);
            }
            
            Initialization.SpinUp(protocols.ToArray());
            GetterTypeMapping.Warmup();
            MetaInterfaceMapping.Warmup();
            OverrideMaskRegistrations.Warmup();
            
            return _registrations;
        }
    }
}
