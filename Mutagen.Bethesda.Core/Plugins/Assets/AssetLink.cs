using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins.Exceptions;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Assets;

/// <summary>
/// Asset referenced by a record
/// </summary>
public class AssetLinkGetter<TAssetType> : 
    IEquatable<AssetLinkGetter<TAssetType>>,
    IComparable<AssetLinkGetter<TAssetType>>, 
    IAssetLinkGetter<TAssetType>
    where TAssetType : class, IAssetType
{
    protected DataRelativeAssetPath _dataRelativeAssetPath;
    protected string _rawPath;
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
        _dataRelativeAssetPath = DataRelativeAssetPath.NullPath;
        _rawPath = DataRelativeAssetPath.NullPath;
    }

    public AssetLinkGetter(DataRelativeAssetPath dataRelativePath)
    {
        if (dataRelativePath.Path == DataRelativeAssetPath.NullPath)
        {
            _dataRelativeAssetPath = DataRelativeAssetPath.NullPath;
            _rawPath = DataRelativeAssetPath.NullPath;
        }
        else
        {
            AssertHasBaseFolder(dataRelativePath.Path);
            _dataRelativeAssetPath = dataRelativePath;
            _rawPath = ExtractAssertRawPath(_dataRelativeAssetPath);
        }
    }

    public AssetLinkGetter(string path)
    {
        if (path == DataRelativeAssetPath.NullPath)
        {
            _dataRelativeAssetPath = DataRelativeAssetPath.NullPath;
            _rawPath = DataRelativeAssetPath.NullPath;
        }
        else
        {
            _dataRelativeAssetPath = path;
            AssertHasBaseFolder(_dataRelativeAssetPath.Path);
            _rawPath = ExtractAssertRawPath(_dataRelativeAssetPath);
        }
    }

    public AssetLinkGetter(FilePath path)
    {
        if (path.Path == DataRelativeAssetPath.NullPath)
        {
            _dataRelativeAssetPath = DataRelativeAssetPath.NullPath;
            _rawPath = DataRelativeAssetPath.NullPath;
        }
        else
        {
            _dataRelativeAssetPath = path;
            AssertHasBaseFolder(_dataRelativeAssetPath.Path);
            _rawPath = ExtractAssertRawPath(_dataRelativeAssetPath);
        }
    }

    protected static bool HasBaseFolder(string path)
    {
        return path.StartsWith(AssetInstance.BaseFolder,
            DataRelativeAssetPath.PathComparison);
    }

    protected static void AssertHasBaseFolder(string path)
    {
        if (!HasBaseFolder(path))
        {
            throw new AssetPathMisalignedException(path, AssetInstance);
        }
    }

    protected static string ExtractAssertRawPath(DataRelativeAssetPath path)
    {
        if (!path.Path.StartsWith(AssetInstance.BaseFolder,
                DataRelativeAssetPath.PathComparison))
        {
            throw new AssetPathMisalignedException(path.Path, AssetInstance);
        }

        return path.Path.Substring(AssetInstance.BaseFolder.Length + 1);
    }

    public string RawPath => _rawPath;

    public DataRelativeAssetPath DataRelativePath => _dataRelativeAssetPath.Path;

    public string Extension => Path.GetExtension(RawPath);

    public IAssetType Type => AssetInstance;

    public override string ToString()
    {
        return DataRelativePath.ToString();
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj is not IAssetLinkGetter<TAssetType> rhs) return false;

        return RawPath.Equals(rhs.RawPath);
    }

    public virtual bool Equals(AssetLinkGetter<TAssetType>? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return RawPath.Equals(other.RawPath);
    }

    public override int GetHashCode()
    {
        return RawPath.GetHashCode();
    }

    public int CompareTo(AssetLinkGetter<TAssetType>? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;

        return RawPath.CompareTo(other.RawPath);
    }
    
    public bool IsNull => DataRelativePath.IsNull;

    [return: NotNullIfNotNull("asset")]
    public static implicit operator string? (AssetLinkGetter<TAssetType>? asset) => asset?.RawPath;

    [return: NotNullIfNotNull("path")]
    public static implicit operator AssetLinkGetter<TAssetType>?(string? path) => path == null ? null : new(path);

    [return: NotNullIfNotNull("path")]
    public static implicit operator AssetLinkGetter<TAssetType>?(FilePath? path) => path == null ? null : new(path.Value.Path);

    [return: NotNullIfNotNull("path")]
    public static implicit operator AssetLinkGetter<TAssetType>?(DataRelativeAssetPath? path) => path == null ? null : new(path.Value);
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
        : base(DataRelativeAssetPath.NullPath)
    {
    }
    
    public AssetLink(string path)
        : base(path)
    {
    }

    public AssetLink(DataRelativeAssetPath dataRelativePath)
        : base(dataRelativePath)
    {
    }

    public AssetLink(FilePath path)
        : base(path)
    {
    }

    public bool TrySetPath(DataRelativeAssetPath? path)
    {
        if (path == null)
        {
            SetToNull();
            return true;
        }
        
        if (path.Value.Path.StartsWith(AssetInstance.BaseFolder, DataRelativeAssetPath.PathComparison))
        {
            _dataRelativeAssetPath = path.Value;
            _rawPath = ExtractAssertRawPath(path.Value);
            return true;
        }
        
        return false;
    }

    public void SetPath(DataRelativeAssetPath? path)
    {
        if (!TrySetPath(path))
        {
            throw new AssetPathMisalignedException(path?.Path!, AssetInstance);
        }
    }

    public new string RawPath
    {
        get => _rawPath;
        set
        {
            _rawPath = value;
            _dataRelativeAssetPath = new DataRelativeAssetPath(value);
        }
    }

    public AssetLink<TAssetType> ShallowClone()
    {
        return new AssetLink<TAssetType>(RawPath);
    }

    public void SetToNull()
    {
        _dataRelativeAssetPath = DataRelativeAssetPath.NullPath;
        _rawPath = DataRelativeAssetPath.NullPath;
    }

    public override string ToString()
    {
        return DataRelativePath.ToString();
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
                typeof(TAssetType).GetHashCode(),
                _dataRelativeAssetPath.GetHashCode());
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj is not IAssetLinkGetter<TAssetType> rhs) return false;

        return DataRelativePath.Equals(rhs.DataRelativePath);
    }

    public virtual bool Equals(AssetLink<TAssetType>? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return _dataRelativeAssetPath.Equals(other._dataRelativeAssetPath);
    }

    public int CompareTo(AssetLink<TAssetType>? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;

        return _dataRelativeAssetPath.CompareTo(other._dataRelativeAssetPath);
    }

    [return: NotNullIfNotNull("asset")]
    public static implicit operator string?(AssetLink<TAssetType>? asset) => asset?.RawPath!;

    [return: NotNullIfNotNull("path")]
    public static implicit operator AssetLink<TAssetType>?(string? path) => path == null ? null : new(path);

    [return: NotNullIfNotNull("path")]
    public static implicit operator AssetLink<TAssetType>?(FilePath? path) => path == null ? null : new(path.Value.Path);

    [return: NotNullIfNotNull("path")]
    public static implicit operator AssetLink<TAssetType>?(DataRelativeAssetPath? path) => path == null ? null : new(path.Value);
}
