using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Fallout4.Assets;

public class Fallout4InterfaceAssetType : IAssetType
{
    public static IAssetType Instance { get; } = new Fallout4InterfaceAssetType();
    public string BaseFolder => "Interface";
    public IEnumerable<string> FileExtensions => [".swf"];
}
