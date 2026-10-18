using Loqui;

namespace Mutagen.Bethesda.Plugins;

/// <summary>
/// A game's statically linked registrations, generated alongside its records.
/// </summary>
internal interface IGameModule
{
    GameCategory Category { get; }

    /// <summary>The game's Loqui protocol.</summary>
    IProtocolRegistration Protocol { get; }
}
