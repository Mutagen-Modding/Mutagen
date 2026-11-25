using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Fallout4.Assets;

public class Fallout4SoundAssetType : IAssetType
{
    public static IAssetType Instance { get; } = new Fallout4SoundAssetType();
    public string BaseFolder => "Sound";
    public IEnumerable<string> FileExtensions => [".wav", ".xwm", ".fuz"];
}
