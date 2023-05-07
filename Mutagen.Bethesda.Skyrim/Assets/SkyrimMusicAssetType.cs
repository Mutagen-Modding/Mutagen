using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Skyrim.Assets;

public class SkyrimMusicAssetType : IAssetType
{
#if NET7_0
    public static IAssetType Instance { get; } = new SkyrimMusicAssetType();
#else
    public static readonly SkyrimMusicAssetType Instance = new();
#endif
    public string BaseFolder => "Music";
    public IEnumerable<string> FileExtensions => new []{ ".wav", ".xwm" };
}