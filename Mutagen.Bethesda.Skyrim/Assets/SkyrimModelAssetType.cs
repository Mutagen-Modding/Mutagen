using System;
using System.Collections.Generic;
using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Skyrim.Assets;

public class SkyrimModelAssetType : IAssetType
{
    public static readonly SkyrimModelAssetType Instance = new();
    public string BaseFolder => throw new NotImplementedException();
    public IEnumerable<string> FileExtensions => throw new NotImplementedException();
}