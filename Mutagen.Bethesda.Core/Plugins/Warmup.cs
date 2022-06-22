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
            
            List<IProtocolRegistration> protocols = new()
            {
                new ProtocolDefinition_Bethesda()
            };

            foreach (var category in EnumExt<GameCategory>.Values)
            {
                try
                {
                    var assemblyName = $"Mutagen.Bethesda.{category}";
                    var obj = Activator.CreateInstance(
                        assemblyName,
                        $"Loqui.ProtocolDefinition_{category}");
                    var regis = obj?.Unwrap() as IProtocolRegistration;
                    if (regis == null) continue;
                    protocols.Add(regis);
                    _registrations.Add(category);
                }
                catch
                {
                }
            }
            
            Initialization.SpinUp(protocols.ToArray());
            MetaInterfaceMapping.Warmup();
            OverrideMaskRegistrations.Warmup();
            _warmedUp = true;
            
            return _registrations;
        }
    }
}
