using System;
using System.IO;

namespace Mutagen.Bethesda.Assets;

/// <summary>
/// Asset referenced by a record
/// </summary>
public class AssetLinkGetter<TAssetType> : IComparable<AssetLinkGetter<TAssetType>>, IAssetLinkGetter<TAssetType>
    where TAssetType : IAssetType
{
    protected string _rawPath;
    
    public AssetLinkGetter(TAssetType assetType, string rawPath)
    {
        _rawPath = rawPath;
        AssetType = assetType;
    }
    
    public AssetLinkGetter(TAssetType assetType)
    {
        _rawPath = string.Empty;
        AssetType = assetType;
    }

    public string RawPath => _rawPath;
    public TAssetType AssetType { get; }

    public string DataRelativePath => Path.Combine(AssetType.BaseFolder, RawPath);

    public string Extension => Path.GetExtension(RawPath).TrimStart('.');

    public override string ToString()
    {
        return DataRelativePath;
    }

    public virtual bool Equals(AssetLinkGetter<TAssetType>? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return DataRelativePath == other.DataRelativePath;
    }

    public override int GetHashCode()
    {
        return RawPath.GetHashCode();
    }

    public int CompareTo(AssetLinkGetter<TAssetType>? other)
    {
        if (ReferenceEquals(this, other)) return 0;

        return ReferenceEquals(null, other) ? 1 : string.Compare(RawPath, other.RawPath, StringComparison.Ordinal);
    }
    
    public bool IsNull => RawPath == string.Empty;
}

/// <summary>
/// Asset referenced by a record
/// </summary>
public class AssetLink<TAssetType> : AssetLinkGetter<TAssetType>, IComparable<AssetLink<TAssetType>>, IAssetLink<TAssetType>
    where TAssetType : IAssetType
{
    public AssetLink(TAssetType assetType, string rawPath)
        : base(assetType, rawPath)
    {
    }
    
    public AssetLink(TAssetType assetType) 
        : base(assetType)
    {
    }

    public new string RawPath => _rawPath;

    public new string DataRelativePath => RawPath.StartsWith(AssetType.BaseFolder)
        ? RawPath
        : Path.Combine(AssetType.BaseFolder, RawPath);

    public new string Extension => Path.GetExtension(RawPath).TrimStart('.');

    public  bool TrySetPath(string? path)
    {
        if (path == null)
        {
            SetToNull();
            return true;
        }
        
        if (path.StartsWith(AssetType.BaseFolder, IAssetPath.PathComparison))
        {
            _rawPath = path[AssetType.BaseFolder.Length..];
            return true;
        }
        
        return false;
    }

    public void SetToNull()
    {
        RawPath = string.Empty;
    }

    public override string ToString()
    {
        return DataRelativePath;
    }

    public virtual bool Equals(AssetLink<TAssetType>? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return DataRelativePath == other.DataRelativePath;
    }

    public int CompareTo(AssetLink<TAssetType>? other)
    {
        if (ReferenceEquals(this, other)) return 0;

        return ReferenceEquals(null, other) ? 1 : string.Compare(RawPath, other.RawPath, StringComparison.Ordinal);
    }
}