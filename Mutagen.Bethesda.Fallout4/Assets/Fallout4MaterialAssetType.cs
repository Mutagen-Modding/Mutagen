using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Fallout4.Assets;

public class Fallout4MaterialAssetType : IAssetType
{
    public static IAssetType Instance { get; } = new Fallout4MaterialAssetType();
    public string BaseFolder => "Materials";
    public IEnumerable<string> FileExtensions => [".bgsm", ".bgem"];
}
