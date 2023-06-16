using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Skyrim.Assets;

public class SkyrimScriptSourceAssetType : IAssetType
{
#if NET7_0_OR_GREATER
    public static IAssetType Instance { get; } = new SkyrimScriptSourceAssetType();
#else
    public static readonly SkyrimScriptSourceAssetType Instance = new();
#endif
    public const string PscExtension = ".psc";
    public string BaseFolder => Path.Combine("Scripts", "Source");
    public IEnumerable<string> FileExtensions => new []{ PscExtension };
}