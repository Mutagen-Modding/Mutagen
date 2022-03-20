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
        _rawPath = IAssetPath.NullPath;
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
        return IAssetPath.PathComparer.GetHashCode(RawPath);
    }

    public int CompareTo(AssetLinkGetter<TAssetType>? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;

        return IAssetPath.PathComparer.Compare(RawPath, other.RawPath);
    }
    
    public bool IsNull => RawPath == IAssetPath.NullPath;
    IAssetType IAssetLinkGetter.AssetType => AssetType;
}

/// <summary>
/// Asset referenced by a record
/// </summary>
public class AssetLink<TAssetType> : 
    AssetLinkGetter<TAssetType>, 
    IComparable<AssetLink<TAssetType>>,
    IAssetLink<AssetLink<TAssetType>, TAssetType>,
    IAssetLink<TAssetType>
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

    public new string RawPath
    {
        get => _rawPath;
        set => _rawPath = value;
    }

    IAssetLink<TAssetType> IAssetLink<IAssetLink<TAssetType>, TAssetType>.ShallowClone()
    {
        return ShallowClone();
    }

    public AssetLink<TAssetType> ShallowClone()
    {
        return new AssetLink<TAssetType>(AssetType, RawPath);
    }

    public void SetToNull()
    {
        _rawPath = IAssetPath.NullPath;
    }

    public override string ToString()
    {
        return DataRelativePath;
    }

    public virtual bool Equals(AssetLink<TAssetType>? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return IAssetPath.PathComparer.Equals(RawPath, other.RawPath);
    }

    public int CompareTo(AssetLink<TAssetType>? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;

        return IAssetPath.PathComparer.Compare(RawPath, other.RawPath);
    }
}
