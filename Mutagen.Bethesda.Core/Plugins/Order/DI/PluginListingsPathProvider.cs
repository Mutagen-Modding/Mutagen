using Mutagen.Bethesda.Environments.DI;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Order.DI;

public interface IPluginListingsPathProvider
{
    /// <summary>
    /// Returns expected location of the plugin load order file
    /// </summary>
    /// <returns>Expected path to load order file</returns>
    FilePath Path { get; }
}

public sealed class PluginListingsPathProvider : IPluginListingsPathProvider
{
    private readonly IGameReleaseContext _gameReleaseContext;

    public PluginListingsPathProvider(IGameReleaseContext gameReleaseContext)
    {
        _gameReleaseContext = gameReleaseContext;
    }
    
    private string GetRelativePluginsPath()
    {
        return _gameReleaseContext.Release switch
        {
            GameRelease.Oblivion => "Oblivion/Plugins.txt",
            GameRelease.SkyrimLE => "Skyrim/Plugins.txt",
            GameRelease.EnderalLE => "Enderal/Plugins.txt",
            GameRelease.SkyrimSE => "Skyrim Special Edition/Plugins.txt",
            GameRelease.EnderalSE => "Enderal Special Edition/Plugins.txt",
            GameRelease.SkyrimVR => "Skyrim VR/Plugins.txt",
            GameRelease.Fallout4 => "Fallout4/Plugins.txt",
            _ => throw new NotImplementedException()
        };
    }

    /// <inheritdoc />
    public FilePath Path => System.IO.Path.Combine(
        Environment.GetEnvironmentVariable("LocalAppData")!,
        GetRelativePluginsPath());
}

public sealed record PluginListingsPathInjection(FilePath Path) : IPluginListingsPathProvider;
