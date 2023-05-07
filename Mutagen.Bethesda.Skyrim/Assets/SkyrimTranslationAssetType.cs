using System.Collections.Generic;
using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Skyrim.Assets;

public class SkyrimTranslationAssetType : IAssetType
{
#if NET7_0
    public static IAssetType Instance { get; } = new SkyrimTranslationAssetType();
#else
    public static readonly SkyrimTranslationAssetType Instance = new();
#endif
    public string BaseFolder => "Strings";
    public IEnumerable<string> FileExtensions => new []{ ".dlstrings", ".ilstrings", ".strings" };
    
    public static IEnumerable<string> Languages => new []{ 
        "english",
        "french",
        "german",
        "italian",
        "japanese",
        "polish",
        "russian",
        "spanish" 
    };
}