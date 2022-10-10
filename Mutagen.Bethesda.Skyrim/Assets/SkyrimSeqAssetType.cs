using System.Collections.Generic;
using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Skyrim.Assets;

public class SkyrimSeqAssetType : IAssetType
{
    public static readonly SkyrimSeqAssetType Instance = new();
    public string BaseFolder => "Seq";
    public IEnumerable<string> FileExtensions => new []{ "seq" };
}