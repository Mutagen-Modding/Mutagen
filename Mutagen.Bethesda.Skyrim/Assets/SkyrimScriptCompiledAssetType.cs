using System.Collections.Generic;
using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Skyrim.Assets;

public class SkyrimScriptCompiledAssetType : IAssetType
{
    public static readonly SkyrimScriptCompiledAssetType Instance = new();
    public string BaseFolder => "Scripts";
    public IEnumerable<string> FileExtensions => new []{ "wav", "xwm" };
}