using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Fallout4.Assets;

public class Fallout4ProgramAssetType : IAssetType
{
    public static IAssetType Instance { get; } = new Fallout4ProgramAssetType();
    public string BaseFolder => "Programs";
    public IEnumerable<string> FileExtensions => [".swf"];
}
