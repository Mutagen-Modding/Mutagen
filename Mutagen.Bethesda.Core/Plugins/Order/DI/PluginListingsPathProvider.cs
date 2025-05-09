﻿using Noggog;

namespace Mutagen.Bethesda.Plugins.Order.DI;

public interface IPluginListingsPathProvider
{
    FilePath Get(GameRelease release);
}

public class PluginListingsPathProvider : IPluginListingsPathProvider
{
    internal string GetGameFolder(GameRelease release)
    {
        return release switch
        {
            GameRelease.Oblivion => "Oblivion",
            GameRelease.OblivionRE => "Oblivion Remastered",
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
            "Plugins.txt");
    }

    public FilePath Get(GameRelease release) => System.IO.Path.Combine(
        Environment.GetEnvironmentVariable("LocalAppData")!,
        GetRelativePluginsPath(release));
}