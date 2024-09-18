using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Skyrim.Assets;

public class SkyrimScriptCompiledAssetType : IAssetType
{
    public static IAssetType Instance { get; } = new SkyrimScriptCompiledAssetType();
    public const string PexExtension = ".pex";
    public string BaseFolder => "Scripts";
    public IEnumerable<string> FileExtensions => new []{ PexExtension };
}