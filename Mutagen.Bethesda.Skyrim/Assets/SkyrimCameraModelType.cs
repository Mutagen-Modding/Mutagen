using System.Collections.Generic;
using System.IO;
using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Skyrim.Assets;

public class SkyrimCameraModelType : IAssetType
{
    public static readonly SkyrimCameraModelType Instance = new();
    public string BaseFolder => Path.Combine("Meshes", "Camera");
    public IEnumerable<string> FileExtensions => new []{"nif"};
}