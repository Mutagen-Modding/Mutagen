using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Order.DI;

public interface IPluginListingsPathProvider
{
    FilePath? Get(GameRelease release);
}

public class PluginListingsPathProvider : IPluginListingsPathProvider
{
    private readonly IDataDirectoryProvider _dataDirectoryProvider;
    private const string FileName = "Plugins.txt";
    
    public PluginListingsPathProvider(IDataDirectoryProvider dataDirectoryProvider)
    {
        _dataDirectoryProvider = dataDirectoryProvider;
    }
    
    internal string GetGameFolder(GameRelease release)
    {
        return release switch
        {
            GameRelease.Oblivion => "Oblivion",
            GameRelease.SkyrimLE => "Skyrim",
            GameRelease.EnderalLE => "Enderal",
            GameRelease.SkyrimSE => "Skyrim Special Edition",
            GameRelease.SkyrimSEGog => "Skyrim Special Edition GOG",
            GameRelease.EnderalSE => "Enderal Special Edition",
            GameRelease.SkyrimVR => "Skyrim VR",
            GameRelease.Fallout4 => "Fallout4",
            GameRelease.Fallout4VR => "Fallout4VR",
            GameRelease.Starfield => "Starfield",
            _ => throw new NotImplementedException()
        };
    }
    
    private string GetRelativePluginsPath(GameRelease release)
    {
        var gameFolder = GetGameFolder(release);
        return System.IO.Path.Combine(
            gameFolder,
            FileName);
    }

    public FilePath? Get(GameRelease release)
    {
        var constants = GameConstants.Get(release);

        if (constants.PluginsFileInGameFolder)
        {
            return Path.Combine(_dataDirectoryProvider.Path, FileName);
        }
        else
        {
            var localAppData = Environment.GetEnvironmentVariable("LocalAppData");
            if (localAppData == null)
            {
                return null;
            }
            return System.IO.Path.Combine(
                localAppData,
                GetRelativePluginsPath(release));
        }
    }
}