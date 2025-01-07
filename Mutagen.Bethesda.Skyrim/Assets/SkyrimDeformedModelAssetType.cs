using System.Collections.Generic;
using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Skyrim.Assets;

public class SkyrimDeformedModelAssetType : IAssetType
{
    public static IAssetType Instance { get; } = new SkyrimDeformedModelAssetType();
    public string BaseFolder => "Meshes";
    public IEnumerable<string> FileExtensions => new []{".tri"};
}