using System;

namespace Mutagen.Bethesda.Assets;

/// <summary>
/// Asset referenced by a record
/// </summary>
public class AssetLinkGetter<TAssetType> : IComparable<AssetLinkGetter<TAssetType>>, IAssetLinkGetter<TAssetType>
    where TAssetType : IAssetType
{
    protected string _rawPath;
    protected string _path;
    
    public AssetLinkGetter(TAssetType assetType, string path)
    {
        _rawPath = path;
        _path = GetDataRelativePath(path);
        AssetType = assetType;
    }
    
    public AssetLinkGetter(TAssetType assetType)
    {
        _rawPath = _path = string.Empty;
        AssetType = assetType;
    }

    public TAssetType AssetType { get; }

    public string Path => _path;

    public string Extension => System.IO.Path.GetExtension(Path).TrimStart('.');

    public override string ToString()
    {
        return Path;
    }

    public virtual bool Equals(AssetLinkGetter<TAssetType>? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return Path == other.Path;
    }

    public override int GetHashCode()
    {
        return IAssetPath.PathComparer.GetHashCode(Path);
    }

    public int CompareTo(AssetLinkGetter<TAssetType>? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;

        return IAssetPath.PathComparer.Compare(Path, other.Path);
    }
    
    public bool IsNull => Path == string.Empty;
    
    protected string GetDataRelativePath(string path)
    {
        return path.StartsWith(AssetType.BaseFolder, IAssetPath.PathComparison) ? path : System.IO.Path.Combine(AssetType.BaseFolder, path);
    }
}

/// <summary>
/// Asset referenced by a record
/// </summary>
public class AssetLink<TAssetType> : AssetLinkGetter<TAssetType>, IComparable<AssetLink<TAssetType>>, IAssetLink<TAssetType>
    where TAssetType : IAssetType
{
    public AssetLink(TAssetType assetType, string path)
        : base(assetType, path)
    {
    }
    
    public AssetLink(TAssetType assetType) 
        : base(assetType)
    {
    }

    public new string Path {
        get => _path;
        set
        {
            _path = value;
            _rawPath = value;
        }
    }

    public new string Extension => System.IO.Path.GetExtension(Path).TrimStart('.');

    public void SetTo(string? path)
    {
        if (path == null)
        {
            SetToNull();
        }
        else
        {
            _rawPath = path;
            Path = GetDataRelativePath(Path);
        }
    }

    public void SetToNull()
    {
        _rawPath = Path = string.Empty;
    }

    public override string ToString()
    {
        return Path;
    }

    public virtual bool Equals(AssetLink<TAssetType>? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return IAssetPath.PathComparer.Equals(Path, other.Path);
    }

    public int CompareTo(AssetLink<TAssetType>? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        
        return IAssetPath.PathComparer.Compare(Path, other.Path);
    }
}