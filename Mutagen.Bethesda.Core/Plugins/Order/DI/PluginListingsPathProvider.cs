using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Installs.DI;
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
    private readonly IGameInstallModeProvider _gameInstallModeProvider;
    private readonly IGameReleaseContext _gameReleaseContext;

    public PluginListingsPathProvider(
        IGameInstallModeProvider gameInstallModeProvider, 
        IGameReleaseContext gameReleaseContext)
    {
        _gameInstallModeProvider = gameInstallModeProvider;
        _gameReleaseContext = gameReleaseContext;
    }

    private string? GetInstallModeSuffix()
    {
        return _gameInstallModeProvider.InstallMode switch
        {
            GameInstallMode.Gog => " GOG",
            _ => null,
        };
    }
    
    private string GetGameFolder()
    {
        return _gameReleaseContext.Release switch
        {
            GameRelease.Oblivion => "Oblivion",
            GameRelease.SkyrimLE => "Skyrim",
            GameRelease.EnderalLE => "Enderal",
            GameRelease.SkyrimSE => "Skyrim Special Edition",
            GameRelease.EnderalSE => "Enderal Special Edition",
            GameRelease.SkyrimVR => "Skyrim VR",
            GameRelease.Fallout4 => "Fallout4",
            _ => throw new NotImplementedException()
        };
    }
    
    private string GetRelativePluginsPath()
    {
        var installModeSuffix = GetInstallModeSuffix();
        var gameFolder = GetGameFolder();
        if (installModeSuffix == null)
        {
            return System.IO.Path.Combine(
                gameFolder,
                "Plugins.txt");
        }
        else
        {
            return System.IO.Path.Combine(
                $"{gameFolder}{installModeSuffix}",
                "Plugins.txt");
        }
    }

    /// <inheritdoc />
    public FilePath Path => System.IO.Path.Combine(
        Environment.GetEnvironmentVariable("LocalAppData")!,
        GetRelativePluginsPath());
}

public sealed record PluginListingsPathInjection(FilePath Path) : IPluginListingsPathProvider;
