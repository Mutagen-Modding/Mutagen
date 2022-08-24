using System.Reflection;

namespace Mutagen.Bethesda.Assets;

/// <summary>
/// Asset referenced by a record
/// </summary>
public class AssetLinkGetter<TAssetType> : IComparable<AssetLinkGetter<TAssetType>>, IAssetLinkGetter<TAssetType>
    where TAssetType : IAssetType
{
    protected string _rawPath;
    protected static readonly TAssetType AssetInstance;

    static AssetLinkGetter()
    {
        AssetInstance = (TAssetType)(typeof(TAssetType).GetProperty("Instance", BindingFlags.Static)?.GetValue(null) ?? Activator.CreateInstance(typeof(TAssetType)))!;
    }
    
    public AssetLinkGetter()
    {
        _rawPath = IAssetLinkGetter.NullPath;
    }

    public AssetLinkGetter(string rawPath)
    {
        _rawPath = rawPath;
    }

    public string RawPath => _rawPath;

    public string DataRelativePath => Path.Combine(AssetInstance.BaseFolder, RawPath);

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
        return IAssetLinkGetter.PathComparer.GetHashCode(RawPath);
    }

    public int CompareTo(AssetLinkGetter<TAssetType>? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;

        return IAssetLinkGetter.PathComparer.Compare(RawPath, other.RawPath);
    }
    
    public bool IsNull => RawPath == IAssetLinkGetter.NullPath;
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
    public AssetLink()
        : base(IAssetLinkGetter.NullPath)
    {
    }
    
    public AssetLink(string rawPath)
        : base(rawPath)
    {
    }

    public bool TrySetPath(string? path)
    {
        if (path == null)
        {
            SetToNull();
            return true;
        }
        
        if (path.StartsWith(AssetInstance.BaseFolder, IAssetLinkGetter.PathComparison))
        {
            _rawPath = path[AssetInstance.BaseFolder.Length..];
            return true;
        }
        
        return false;
    }

    public new string RawPath
    {
        get => _rawPath;
        set => _rawPath = value;
    }

    public AssetLink<TAssetType> ShallowClone()
    {
        return new AssetLink<TAssetType>(RawPath);
    }

    public void SetToNull()
    {
        _rawPath = IAssetLinkGetter.NullPath;
    }

    public override string ToString()
    {
        return DataRelativePath;
    }

    public virtual bool Equals(AssetLink<TAssetType>? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return IAssetLinkGetter.PathComparer.Equals(RawPath, other.RawPath);
    }

    public int CompareTo(AssetLink<TAssetType>? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;

        return IAssetLinkGetter.PathComparer.Compare(RawPath, other.RawPath);
    }
}
