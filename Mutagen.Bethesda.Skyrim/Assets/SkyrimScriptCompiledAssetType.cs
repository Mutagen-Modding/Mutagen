using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Skyrim.Assets;

public class SkyrimScriptCompiledAssetType : IAssetType
{
#if NET7_0
    public static IAssetType Instance { get; } = new SkyrimScriptCompiledAssetType();
#else
    public static readonly SkyrimScriptCompiledAssetType Instance = new();
#endif
    public const string PexExtension = ".pex";
    public string BaseFolder => "Scripts";
    public IEnumerable<string> FileExtensions => new []{ PexExtension };
}