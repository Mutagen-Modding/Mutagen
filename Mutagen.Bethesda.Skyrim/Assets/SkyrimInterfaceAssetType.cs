using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Skyrim.Assets;

public class SkyrimInterfaceAssetType : IAssetType
{
    public static IAssetType Instance { get; } = new SkyrimInterfaceAssetType();
    public string BaseFolder => "Interface";
    public IEnumerable<string> FileExtensions => new []{ ".swf", ".gfx" };
}