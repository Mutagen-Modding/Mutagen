using System.Diagnostics.CodeAnalysis;
using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Skyrim;

/// <summary>
/// Supplies Skyrim's generated registrations to Core before its first lookup.
/// </summary>
public static class GameRegistration
{
    private static readonly GameRegistrationDefinition Definition = new()
    {
        Protocol = new ProtocolDefinition_Skyrim(),
        Mod = SkyrimMod_Registration.Instance,
        AspectMapping = static () => new SkyrimAspectInterfaceMapping(),
        InheritingMapping = static () => new SkyrimInheritingInterfaceMapping(),
        LinkMapping = static () => new SkyrimLinkInterfaceMapping(),
        AbstractMapping = static () => new SkyrimIsolatedAbstractInterfaceMapping(),
        OverrideMasks = static () => new SkyrimOverrideMaskRegistration(),
    };

    /// <summary>
    /// Registers Skyrim before using Core game registration, warmup, or interface mapping APIs.
    /// Repeated calls are safe; a first call after Core's first lookup throws.
    /// </summary>
    // The protocol references open generic groups; trimming must retain their inherited interface methods.
    [DynamicDependency("GetEnumerator", typeof(AGroup<>))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicMethods, typeof(AGroup<>))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicMethods, typeof(AListGroup<>))]
    public static void Register()
    {
        GameRegistrations.Register(GameCategory.Skyrim, Definition);
    }
}