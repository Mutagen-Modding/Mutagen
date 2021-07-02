using System;
using Mutagen.Bethesda.Environments;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Order
{
    public interface IPluginPathContext
    {
        /// <summary>
        /// Returns expected location of the plugin load order file
        /// </summary>
        /// <returns>Expected path to load order file</returns>
        FilePath Path { get; }
    }

    public class PluginPathContext : IPluginPathContext
    {
        private readonly IGameReleaseContext _gameReleaseContext;

        public PluginPathContext(IGameReleaseContext gameReleaseContext)
        {
            _gameReleaseContext = gameReleaseContext;
        }
        
        private string GetRelativePluginsPath()
        {
            return _gameReleaseContext.Release switch
            {
                GameRelease.Oblivion => "Oblivion/Plugins.txt",
                GameRelease.SkyrimLE => "Skyrim/Plugins.txt",
                GameRelease.SkyrimSE => "Skyrim Special Edition/Plugins.txt",
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

    public record PluginPathInjection(FilePath Path) : IPluginPathContext;
}