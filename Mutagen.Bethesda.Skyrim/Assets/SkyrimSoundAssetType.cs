using System.Collections.Generic;
using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Skyrim.Assets;

public class SkyrimSoundAssetType : IAssetType
{
#if NET7_0_OR_GREATER
    public static IAssetType Instance { get; } = new SkyrimSoundAssetType();
#else
    public static readonly SkyrimSoundAssetType Instance = new();
#endif
    public string BaseFolder => "Sound";
    public IEnumerable<string> FileExtensions => new []{ ".wav", ".xwm" };
}