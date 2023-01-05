using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Plugins.Exceptions;

public class AssetPathMisalignedException : Exception
{
    public string Path { get; }
    public IAssetType AssetType { get; }
    
    public AssetPathMisalignedException(IAssetType assetType, string path)
        : base($"Path did not start with expected Asset folder prefix \"{assetType.BaseFolder}\"")
    {
        Path = path;
        AssetType = assetType;
    }
}