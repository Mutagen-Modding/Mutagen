using Loqui;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins.Cache.Internals;
using Mutagen.Bethesda.Plugins.Records.Mapping;

namespace Mutagen.Bethesda.Plugins;

/// <summary>
/// A game's statically linked registrations, generated alongside its records.
/// </summary>
internal interface IGameModule
{
    GameCategory Category { get; }

    /// <summary>The game's Loqui protocol.</summary>
    IProtocolRegistration Protocol { get; }

    ILoquiRegistration ModRegistration { get; }

    IInterfaceMapping CreateAspectInterfaceMapping();

    IInterfaceMapping CreateLinkInterfaceMapping();

    IInterfaceMapping CreateInheritingInterfaceMapping();

    IInterfaceMapping CreateIsolatedAbstractInterfaceMapping();

    IOverrideMaskRegistration CreateOverrideMaskRegistration();

    IReadOnlyList<IAssetType> AssetTypes { get; }
}
