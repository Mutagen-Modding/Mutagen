using System;
using System.IO;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Order
{
    public interface IPluginPathProvider
    {
        /// <summary>
        /// Returns expected location of the plugin load order file
        /// </summary>
        /// <param name="release">Release to query</param>
        /// <returns>Expected path to load order file</returns>
        FilePath Get(GameRelease release);
    }

    public class PluginPathProvider : IPluginPathProvider
    {
        private string GetRelativePluginsPath(GameRelease release)
        {
            return release switch
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
        public FilePath Get(GameRelease release)
        {
            string pluginPath = GetRelativePluginsPath(release);
            return Path.Combine(
                Environment.GetEnvironmentVariable("LocalAppData")!,
                pluginPath);
        }
    }
}