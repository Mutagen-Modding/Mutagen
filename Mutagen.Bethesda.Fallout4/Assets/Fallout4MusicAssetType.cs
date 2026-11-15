using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Fallout4.Assets;

public class Fallout4MusicAssetType : IAssetType
{
    public static IAssetType Instance { get; } = new Fallout4MusicAssetType();
    public string BaseFolder => "Music";
    public IEnumerable<string> FileExtensions => [".wav", ".xwm"];
}
