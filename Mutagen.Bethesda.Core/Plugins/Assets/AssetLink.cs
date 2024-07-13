using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins.Exceptions;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Assets;

/// <summary>
/// Asset referenced by a record
/// </summary>
public class AssetLinkGetter<TAssetType> : IEquatable<AssetLinkGetter<TAssetType>>, IComparable<AssetLinkGetter<TAssetType>>, IAssetLinkGetter<TAssetType>
    where TAssetType : class, IAssetType
{
    protected AssetPath _assetPath;
    protected static readonly IAssetType AssetInstance;
    public static readonly AssetLinkGetter<TAssetType> Null = new();

    static AssetLinkGetter()
    {
#if NET7_0_OR_GREATER
        AssetInstance = TAssetType.Instance;
#else
        AssetInstance = (TAssetType)(typeof(TAssetType).GetProperty("Instance", BindingFlags.Static)?.GetValue(null) ?? Activator.CreateInstance(typeof(TAssetType)))!;
#endif
    }

    public AssetLinkGetter()
    {
        _assetPath = AssetPath.NullPath;
    }

    public AssetLinkGetter(string rawPath)
    {
        _assetPath = rawPath;
    }

    public AssetPath AssetPath => _assetPath;

    public string RawPath => _assetPath.RawPath;

    public string DataRelativePath => _assetPath.DataRelativePath;

    public string Extension => Path.GetExtension(RawPath);

    public IAssetType Type => AssetInstance;

    public override string ToString()
    {
        return AssetPath.ToString();
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj is not IAssetLinkGetter<TAssetType> rhs) return false;

        return AssetPath.Equals(rhs.AssetPath);
    }

    public virtual bool Equals(AssetLinkGetter<TAssetType>? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return AssetPath.Equals(other.AssetPath);
    }

    public override int GetHashCode()
    {
        return AssetPath.GetHashCode();
    }

    public int CompareTo(AssetLinkGetter<TAssetType>? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;

        return AssetPath.CompareTo(other.AssetPath);
    }
    
    public bool IsNull => AssetPath.IsNull;

    [return: NotNullIfNotNull("asset")]
    public static implicit operator string? (AssetLinkGetter<TAssetType>? asset) => asset?.RawPath;

    [return: NotNullIfNotNull("path")]
    public static implicit operator AssetLinkGetter<TAssetType>?(string? path) => path == null ? null : new(path);

    [return: NotNullIfNotNull("path")]
    public static implicit operator AssetLinkGetter<TAssetType>?(FilePath? path) => path == null ? null : new(path.Value.Path);

    [return: NotNullIfNotNull("path")]
    public static implicit operator AssetLinkGetter<TAssetType>?(AssetPath? path) => path == null ? null : new(path.Value.RawPath);
}

/// <summary>
/// Asset referenced by a record
/// </summary>
public class AssetLink<TAssetType> : 
    AssetLinkGetter<TAssetType>, 
    IEquatable<AssetLink<TAssetType>>,
    IComparable<AssetLink<TAssetType>>,
    IAssetLink<AssetLink<TAssetType>, TAssetType>,
    IAssetLink<TAssetType>
    where TAssetType : class, IAssetType
{
    public AssetLink()
        : base(AssetPath.NullPath)
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
        
        if (path.StartsWith(AssetInstance.BaseFolder, AssetPath.PathComparison))
        {
            _assetPath = path[AssetInstance.BaseFolder.Length..];
            _assetPath = _assetPath.RawPath
                .TrimStart(Path.DirectorySeparatorChar)
                .TrimStart(Path.AltDirectorySeparatorChar);
            return true;
        }
        
        return false;
    }

    public void SetPath(string? path)
    {
        if (!TrySetPath(path))
        {
            throw new AssetPathMisalignedException(AssetInstance, path!);
        }
    }

    public new string RawPath
    {
        get => _assetPath.RawPath;
        set => _assetPath = value;
    }

    public AssetLink<TAssetType> ShallowClone()
    {
        return new AssetLink<TAssetType>(RawPath);
    }

    public void SetToNull()
    {
        _assetPath = AssetPath.NullPath;
    }

    public override string ToString()
    {
        return AssetPath.ToString();
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
                typeof(TAssetType).GetHashCode(),
                _assetPath.GetHashCode());
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj is not IAssetLinkGetter<TAssetType> rhs) return false;

        return AssetPath.Equals(rhs.AssetPath);
    }

    public virtual bool Equals(AssetLink<TAssetType>? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return _assetPath.Equals(other._assetPath);
    }

    public int CompareTo(AssetLink<TAssetType>? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;

        return _assetPath.CompareTo(other._assetPath);
    }

    [return: NotNullIfNotNull("asset")]
    public static implicit operator string?(AssetLink<TAssetType>? asset) => asset?.RawPath!;

    [return: NotNullIfNotNull("path")]
    public static implicit operator AssetLink<TAssetType>?(string? path) => path == null ? null : new(path);

    [return: NotNullIfNotNull("path")]
    public static implicit operator AssetLink<TAssetType>?(FilePath? path) => path == null ? null : new(path.Value.Path);

    [return: NotNullIfNotNull("path")]
    public static implicit operator AssetLink<TAssetType>?(AssetPath? path) => path == null ? null : new(path.Value.RawPath);
}
