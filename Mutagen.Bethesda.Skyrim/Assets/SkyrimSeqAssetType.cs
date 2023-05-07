using System.Collections.Generic;
using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Skyrim.Assets;

public class SkyrimSeqAssetType : IAssetType
{
#if NET7_0
    public static IAssetType Instance { get; } = new SkyrimSeqAssetType();
#else
    public static readonly SkyrimSeqAssetType Instance = new();
#endif
    public string BaseFolder => "Seq";
    public IEnumerable<string> FileExtensions => new []{ ".seq" };
}