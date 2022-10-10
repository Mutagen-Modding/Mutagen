using System.Collections.Generic;
using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Skyrim.Assets;

public class SkyrimMusicAssetType : IAssetType
{
    public static readonly SkyrimMusicAssetType Instance = new();
    public string BaseFolder => "Music";
    public IEnumerable<string> FileExtensions => new []{ "wav", "xwm" };
}