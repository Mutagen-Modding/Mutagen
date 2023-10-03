using System.ComponentModel;

namespace Mutagen.Bethesda;

/// <summary>
/// Game categories that generally have similar or the same formats
/// </summary>
public enum GameCategory
{
    [Description("Oblivion")]
    Oblivion,
    [Description("Skyrim")]
    Skyrim, 
    [Description("Fallout4")]
    Fallout4,
    [Description("Starfield")]
    Starfield,
}

public static class GameReleaseKernelExt
{
    public static GameCategory ToCategory(this GameRelease release)
    {
        return release switch
        {
            GameRelease.Oblivion => GameCategory.Oblivion,
            GameRelease.SkyrimLE => GameCategory.Skyrim,
            GameRelease.SkyrimSE => GameCategory.Skyrim,
            GameRelease.SkyrimSEGog => GameCategory.Skyrim,
            GameRelease.SkyrimVR => GameCategory.Skyrim,
            GameRelease.EnderalLE => GameCategory.Skyrim,
            GameRelease.EnderalSE => GameCategory.Skyrim,
            GameRelease.Fallout4 => GameCategory.Fallout4,
            GameRelease.Starfield => GameCategory.Starfield,
            _ => throw new NotImplementedException(),
        };
    }
    
    public static ushort? GetDefaultFormVersion(this GameRelease release)
    {
        return release switch
        {
            GameRelease.Oblivion => default,
            GameRelease.SkyrimLE => 43,
            GameRelease.EnderalLE => 43,
            GameRelease.SkyrimSE => 44,
            GameRelease.SkyrimSEGog => 44,
            GameRelease.EnderalSE => 44,
            GameRelease.SkyrimVR => 44,
            GameRelease.Fallout4 => 131,
            GameRelease.Starfield => 555,
            _ => throw new NotImplementedException(),
        };
    }
}
