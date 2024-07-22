using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Plugins.Exceptions;

public class AssetPathMisalignedException : Exception
{
    public string Path { get; }
    public IAssetType? AssetType { get; }
    
    public AssetPathMisalignedException(string path, IAssetType assetType)
        : base($"Path did not start with expected Asset folder prefix \"{assetType.BaseFolder}\"")
    {
        Path = path;
        AssetType = assetType;
    }
    
    public AssetPathMisalignedException(string path, string message)
        : base(message)
    {
        Path = path;
        AssetType = null;
    }
}