using Noggog;

namespace Mutagen.Bethesda.Plugins.Order.DI;

public interface IPluginListingsPathProvider
{
    FilePath Get(GameRelease release, GameInstallMode installMode);
}

public class PluginListingsPathProvider : IPluginListingsPathProvider
{
    private string? GetInstallModeSuffix(GameInstallMode installMode)
    {
        return installMode switch
        {
            GameInstallMode.Gog => " GOG",
            _ => null,
        };
    }
    
    private string GetGameFolder(GameRelease release)
    {
        return release switch
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
    
    private string GetRelativePluginsPath(GameRelease release, GameInstallMode installMode)
    {
        var installModeSuffix = GetInstallModeSuffix(installMode);
        var gameFolder = GetGameFolder(release);
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

    public FilePath Get(GameRelease release, GameInstallMode installMode) => System.IO.Path.Combine(
        Environment.GetEnvironmentVariable("LocalAppData")!,
        GetRelativePluginsPath(release, installMode));
}