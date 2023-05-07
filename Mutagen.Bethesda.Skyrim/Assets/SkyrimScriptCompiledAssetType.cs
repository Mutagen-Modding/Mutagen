using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Skyrim.Assets;

public class SkyrimScriptCompiledAssetType : IAssetType
{
#if NET7_0_OR_GREATER
    public static IAssetType Instance { get; } = new SkyrimScriptCompiledAssetType();
#else
    public static readonly SkyrimScriptCompiledAssetType Instance = new();
#endif
    public const string PexExtension = ".pex";
    public string BaseFolder => "Scripts";
    public IEnumerable<string> FileExtensions => new []{ PexExtension };
}