using System.Diagnostics.CodeAnalysis;
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
    protected DataRelativePath _dataRelativePath;
    protected string _givenPath;
    protected static readonly TAssetType AssetInstance;
    public static readonly AssetLinkGetter<TAssetType> Null = new();

    public IAssetType AssetTypeInstance => AssetInstance;

    static AssetLinkGetter()
    {
        AssetInstance = (TAssetType)TAssetType.Instance;
    }

    public AssetLinkGetter()
    {
        _dataRelativePath = DataRelativePath.NullPath;
        _givenPath = DataRelativePath.NullPath;
    }

    public AssetLinkGetter(DataRelativePath dataRelativePath)
    {
        if (dataRelativePath.Path == DataRelativePath.NullPath)
        {
            _dataRelativePath = DataRelativePath.NullPath;
            _givenPath = DataRelativePath.NullPath;
        }
        else
        {
            AssertHasBaseFolder(dataRelativePath.Path);
            _givenPath = dataRelativePath.Path;
            _dataRelativePath = dataRelativePath;
        }
    }

    public AssetLinkGetter(string path)
    {
        if (path == DataRelativePath.NullPath)
        {
            _dataRelativePath = DataRelativePath.NullPath;
            _givenPath = DataRelativePath.NullPath;
        }
        else
        {
            _givenPath = path;
            _dataRelativePath = GetDataRelativePath(path);
        }
    }

    public AssetLinkGetter(FilePath path)
    {
        if (path.Path == DataRelativePath.NullPath)
        {
            _dataRelativePath = DataRelativePath.NullPath;
            _givenPath = DataRelativePath.NullPath;
        }
        else
        {
            _givenPath = path;
            _dataRelativePath = GetDataRelativePath(path.Path);
        }
    }

    protected static DataRelativePath GetDataRelativePath(string path)
    {
        if (DataRelativePath.HasDataDirectory(path))
        {
            var dataRelativePath = new DataRelativePath(path);
            AssertHasBaseFolder(dataRelativePath.Path);
            return dataRelativePath;
        }

        if (!HasBaseFolder(path))
        {
            path = Path.Combine(AssetInstance.BaseFolder, path);
        }

        // No check if the data relative path has a base folder is needed here
        return new DataRelativePath(path);
    }

    protected static bool HasBaseFolder(string path)
    {
        return path.StartsWith(AssetInstance.BaseFolder,
            DataRelativePath.PathComparison);
    }

    protected static void AssertHasBaseFolder(string path)
    {
        if (!HasBaseFolder(path))
        {
            throw new AssetPathMisalignedException(path, AssetInstance);
        }
    }

    protected static string ExtractAssertBaseFolderSubpath(DataRelativePath path)
    {
        if (!path.Path.StartsWith(AssetInstance.BaseFolder,
                DataRelativePath.PathComparison))
        {
            throw new AssetPathMisalignedException(path.Path, AssetInstance);
        }

        return path.Path.Substring(AssetInstance.BaseFolder.Length + 1);
    }

    public string GivenPath => _givenPath;

    public DataRelativePath DataRelativePath => _dataRelativePath.Path;

    public string Extension => Path.GetExtension(GivenPath);

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

        return DataRelativePath.Equals(rhs.DataRelativePath);
    }

    public virtual bool Equals(AssetLinkGetter<TAssetType>? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return DataRelativePath.Equals(other.DataRelativePath);
    }

    public override int GetHashCode()
    {
        return _dataRelativePath.GetHashCode();
    }

    public int CompareTo(AssetLinkGetter<TAssetType>? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;

        return DataRelativePath.CompareTo(other.DataRelativePath);
    }

    public bool IsNull => DataRelativePath.IsNull;

    [return: NotNullIfNotNull("asset")]
    public static implicit operator string?(AssetLinkGetter<TAssetType>? asset) => asset?.GivenPath;

    [return: NotNullIfNotNull("path")]
    public static implicit operator AssetLinkGetter<TAssetType>?(string? path) => path == null ? null : new(path);

    [return: NotNullIfNotNull("path")]
    public static implicit operator AssetLinkGetter<TAssetType>?(FilePath? path) => path == null ? null : new(path.Value.Path);

    [return: NotNullIfNotNull("path")]
    public static implicit operator AssetLinkGetter<TAssetType>?(DataRelativePath? path) => path == null ? null : new(path.Value);
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
    TAssetType IAssetLink<TAssetType>.AssetTypeInstance => AssetInstance;

    public AssetLink()
        : base(DataRelativePath.NullPath)
    {
    }

    public AssetLink(string path)
        : base(path)
    {
    }

    public AssetLink(DataRelativePath dataRelativePath)
        : base(dataRelativePath)
    {
    }

    public AssetLink(FilePath path)
        : base(path)
    {
    }

    public bool TrySetPath(DataRelativePath? path)
    {
        if (path == null)
        {
            SetToNull();
            return true;
        }

        if (path.Value.Path.StartsWith(AssetInstance.BaseFolder, DataRelativePath.PathComparison))
        {
            _dataRelativePath = path.Value;
            _givenPath = path.Value.Path;
            return true;
        }

        return false;
    }

    public bool TrySetPath(string? path)
    {
        if (path == null)
        {
            SetToNull();
            return true;
        }

        DataRelativePath dataRelativePath = path;

        if (dataRelativePath.Path.StartsWith(AssetInstance.BaseFolder, DataRelativePath.PathComparison))
        {
            _dataRelativePath = dataRelativePath;
            _givenPath = path;
            return true;
        }

        return false;
    }


    public new string GivenPath
    {
        get => _givenPath;
        set
        {
            if (value == DataRelativePath.NullPath)
            {
                _dataRelativePath = DataRelativePath.NullPath;
                _givenPath = DataRelativePath.NullPath;
            }
            else
            {
                _givenPath = value;
                _dataRelativePath = GetDataRelativePath(value);
            }
        }
    }

    public AssetLink<TAssetType> ShallowClone()
    {
        return new AssetLink<TAssetType>(GivenPath);
    }

    public void SetToNull()
    {
        _dataRelativePath = DataRelativePath.NullPath;
        _givenPath = DataRelativePath.NullPath;
    }

    public override string ToString()
    {
        return DataRelativePath.ToString();
    }

    public override int GetHashCode()
    {
        return _dataRelativePath.GetHashCode();
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

        return _dataRelativePath.Equals(other._dataRelativePath);
    }

    public int CompareTo(AssetLink<TAssetType>? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;

        return _dataRelativePath.CompareTo(other._dataRelativePath);
    }

    [return: NotNullIfNotNull("asset")]
    public static implicit operator string?(AssetLink<TAssetType>? asset) => asset?.GivenPath!;

    [return: NotNullIfNotNull("path")]
    public static implicit operator AssetLink<TAssetType>?(string? path) => path == null ? null : new(path);

    [return: NotNullIfNotNull("path")]
    public static implicit operator AssetLink<TAssetType>?(FilePath? path) => path == null ? null : new(path.Value.Path);

    [return: NotNullIfNotNull("path")]
    public static implicit operator AssetLink<TAssetType>?(DataRelativePath? path) => path == null ? null : new(path.Value);
}

public static class AssetLinkExt
{
    public static void SetPath(this IAssetLink assetLink, DataRelativePath? path)
    {
        if (!assetLink.TrySetPath(path))
        {
            throw new AssetPathMisalignedException(path?.Path!, assetLink.AssetTypeInstance);
        }
    }

    public static void SetPath(this IAssetLink assetLink, string? path)
    {
        if (!assetLink.TrySetPath(path))
        {
            throw new AssetPathMisalignedException(path!, assetLink.AssetTypeInstance);
        }
    }
}