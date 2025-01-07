using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Skyrim.Assets;

public class SkyrimScriptSourceAssetType : IAssetType
{
    public static IAssetType Instance { get; } = new SkyrimScriptSourceAssetType();
    public const string PscExtension = ".psc";
    public string BaseFolder => Path.Combine("Scripts", "Source");
    public IEnumerable<string> FileExtensions => new []{ PscExtension };
}