using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Skyrim.Assets;

public class SkyrimInterfaceAssetType : IAssetType
{
#if NET7_0_OR_GREATER
    public static IAssetType Instance { get; } = new SkyrimInterfaceAssetType();
#else
    public static readonly SkyrimInterfaceAssetType Instance = new();
#endif
    public string BaseFolder => "Interface";
    public IEnumerable<string> FileExtensions => new []{ ".swf", ".gfx" };
}