using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Skyrim.Assets;

public class SkyrimScriptSourceAssetType : IAssetType
{
    public static readonly SkyrimScriptSourceAssetType Instance = new();
    public const string PscExtension = ".psc";
    public string BaseFolder => Path.Combine("Scripts", "Source");
    public IEnumerable<string> FileExtensions => new []{ PscExtension };
}