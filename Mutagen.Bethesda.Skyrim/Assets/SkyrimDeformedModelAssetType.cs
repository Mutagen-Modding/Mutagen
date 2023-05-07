using System.Collections.Generic;
using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Skyrim.Assets;

public class SkyrimDeformedModelAssetType : IAssetType
{
#if NET7_0_OR_GREATER
    public static IAssetType Instance { get; } = new SkyrimDeformedModelAssetType();
#else
    public static readonly SkyrimDeformedModelAssetType Instance = new();
#endif
    public string BaseFolder => "Meshes";
    public IEnumerable<string> FileExtensions => new []{".tri"};
}