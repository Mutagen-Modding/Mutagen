using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Skyrim.Assets;

public class SkyrimMusicAssetType : IAssetType
{
    public static IAssetType Instance { get; } = new SkyrimMusicAssetType();
    public string BaseFolder => "Music";
    public IEnumerable<string> FileExtensions => new []{ ".wav", ".xwm" };
}