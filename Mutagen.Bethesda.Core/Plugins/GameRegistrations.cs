using Loqui;
using Mutagen.Bethesda.Plugins.Cache.Internals;
using Mutagen.Bethesda.Plugins.Records.Mapping;

namespace Mutagen.Bethesda.Plugins;

/// <summary>
/// Holds the statically referenced registrations supplied by a game assembly.
/// </summary>
internal sealed class GameRegistrationDefinition
{
    /// <summary>The game's complete Loqui protocol.</summary>
    public required IProtocolRegistration Protocol { get; init; }

    /// <summary>The game's mod registration.</summary>
    public required ILoquiRegistration Mod { get; init; }

    /// <summary>Creates the game's aspect interface mapping.</summary>
    public required Func<IInterfaceMapping> AspectMapping { get; init; }

    /// <summary>Creates the game's inheriting interface mapping.</summary>
    public required Func<IInterfaceMapping> InheritingMapping { get; init; }

    /// <summary>Creates the game's link interface mapping.</summary>
    public required Func<IInterfaceMapping> LinkMapping { get; init; }

    /// <summary>Creates the game's isolated abstract interface mapping.</summary>
    public required Func<IInterfaceMapping> AbstractMapping { get; init; }

    /// <summary>Creates the game's override mask registration.</summary>
    public required Func<IOverrideMaskRegistration> OverrideMasks { get; init; }
}

/// <summary>
/// Collects game registrations until the first lookup builds dependent tables.
/// </summary>
internal static class GameRegistrations
{
    private static readonly Lock SyncRoot = new();
    private static readonly Dictionary<GameCategory, GameRegistrationDefinition> Registrations = new();
    private static bool _frozen;

    /// <summary>Registers a game before the first registration lookup.</summary>
    public static void Register(GameCategory category, GameRegistrationDefinition definition)
    {
        lock (SyncRoot)
        {
            if (Registrations.TryGetValue(category, out var existing))
            {
                if (ReferenceEquals(existing, definition)) return;
                throw new InvalidOperationException($"{category} is already registered.");
            }

            if (_frozen)
            {
                throw new InvalidOperationException($"{category} must be registered before the first game registration lookup.");
            }

            Registrations.Add(category, definition);
        }
    }

    /// <summary>Finds a game registration and closes registration to new games.</summary>
    public static bool TryGet(GameCategory category, out GameRegistrationDefinition definition)
    {
        lock (SyncRoot)
        {
            _frozen = true;
            return Registrations.TryGetValue(category, out definition!);
        }
    }
}