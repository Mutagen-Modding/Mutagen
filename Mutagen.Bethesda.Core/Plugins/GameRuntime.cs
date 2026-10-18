using Noggog;

namespace Mutagen.Bethesda.Plugins;

/// <summary>
/// Locates the generated module of every referenced game library.
/// </summary>
internal static class GameRuntime
{
    private static readonly Lazy<IReadOnlyList<IGameModule>> _modules = new(DiscoverModules);

    /// <summary>The modules of the referenced game libraries, in <see cref="GameCategory"/> order.</summary>
    public static IReadOnlyList<IGameModule> Modules => _modules.Value;

    private static IReadOnlyList<IGameModule> DiscoverModules()
    {
        var modules = new List<IGameModule>();
        foreach (var category in Enums<GameCategory>.Values)
        {
            var moduleType = Type.GetType(
                $"Mutagen.Bethesda.{category}.{category}GameModule, Mutagen.Bethesda.{category}");
            if (moduleType == null) continue;
            modules.Add((IGameModule)Activator.CreateInstance(moduleType)!);
        }
        return modules;
    }
}
