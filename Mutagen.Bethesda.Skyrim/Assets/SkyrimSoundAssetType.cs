using System.Collections.Generic;
using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Skyrim.Assets;

public class SkyrimSoundAssetType : IAssetType
{
    public static readonly SkyrimSoundAssetType Instance = new();
    public string BaseFolder => "Sound";
    public IEnumerable<string> FileExtensions => new []{ ".wav", ".xwm" };
}