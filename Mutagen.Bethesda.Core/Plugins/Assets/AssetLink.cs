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
    protected DataRelativePath _DataRelativePath;
    protected string _givenPath;
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
        _DataRelativePath = Bethesda.Assets.DataRelativePath.NullPath;
        _givenPath = Bethesda.Assets.DataRelativePath.NullPath;
    }

    public AssetLinkGetter(DataRelativePath dataRelativePath)
    {
        if (dataRelativePath.Path == Bethesda.Assets.DataRelativePath.NullPath)
        {
            _DataRelativePath = Bethesda.Assets.DataRelativePath.NullPath;
            _givenPath = Bethesda.Assets.DataRelativePath.NullPath;
        }
        else
        {
            AssertHasBaseFolder(dataRelativePath.Path);
            _givenPath = dataRelativePath.Path;
            _DataRelativePath = dataRelativePath;
        }
    }

    public AssetLinkGetter(string path)
    {
        if (path == Bethesda.Assets.DataRelativePath.NullPath)
        {
            _DataRelativePath = Bethesda.Assets.DataRelativePath.NullPath;
            _givenPath = Bethesda.Assets.DataRelativePath.NullPath;
        }
        else
        {
            _givenPath = path;
            
            if (!Bethesda.Assets.DataRelativePath.HasDataDirectory(path) 
                && !HasBaseFolder(path))
            {
                path = Path.Combine(AssetInstance.BaseFolder, path);
            }

            _DataRelativePath = path;
            AssertHasBaseFolder(_DataRelativePath.Path);
        }
    }

    public AssetLinkGetter(FilePath path)
    {
        if (path.Path == Bethesda.Assets.DataRelativePath.NullPath)
        {
            _DataRelativePath = Bethesda.Assets.DataRelativePath.NullPath;
            _givenPath = Bethesda.Assets.DataRelativePath.NullPath;
        }
        else
        {
            _givenPath = path;
            _DataRelativePath = path;
            AssertHasBaseFolder(_DataRelativePath.Path);
        }
    }

    protected static bool HasBaseFolder(string path)
    {
        return path.StartsWith(AssetInstance.BaseFolder,
            Bethesda.Assets.DataRelativePath.PathComparison);
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
                Bethesda.Assets.DataRelativePath.PathComparison))
        {
            throw new AssetPathMisalignedException(path.Path, AssetInstance);
        }

        return path.Path.Substring(AssetInstance.BaseFolder.Length + 1);
    }

    public string GivenPath => _givenPath;

    public DataRelativePath DataRelativePath => _DataRelativePath.Path;

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
        return DataRelativePath.GetHashCode();
    }

    public int CompareTo(AssetLinkGetter<TAssetType>? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;

        return DataRelativePath.CompareTo(other.DataRelativePath);
    }
    
    public bool IsNull => DataRelativePath.IsNull;

    [return: NotNullIfNotNull("asset")]
    public static implicit operator string? (AssetLinkGetter<TAssetType>? asset) => asset?.GivenPath;

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
            _DataRelativePath = path.Value;
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
            _DataRelativePath = dataRelativePath;
            _givenPath = path;
            return true;
        }
        
        return false;
    }

    public void SetPath(DataRelativePath? path)
    {
        if (!TrySetPath(path))
        {
            throw new AssetPathMisalignedException(path?.Path!, AssetInstance);
        }
    }

    public void SetPath(string? path)
    {
        if (!TrySetPath(path))
        {
            throw new AssetPathMisalignedException(path!, AssetInstance);
        }
    }

    public new string GivenPath
    {
        get => _givenPath;
        set
        {
            _givenPath = value;
            _DataRelativePath = new DataRelativePath(value);
        }
    }

    public AssetLink<TAssetType> ShallowClone()
    {
        return new AssetLink<TAssetType>(GivenPath);
    }

    public void SetToNull()
    {
        _DataRelativePath = DataRelativePath.NullPath;
        _givenPath = DataRelativePath.NullPath;
    }

    public override string ToString()
    {
        return DataRelativePath.ToString();
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
                typeof(TAssetType).GetHashCode(),
                _DataRelativePath.GetHashCode());
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

        return _DataRelativePath.Equals(other._DataRelativePath);
    }

    public int CompareTo(AssetLink<TAssetType>? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;

        return _DataRelativePath.CompareTo(other._DataRelativePath);
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
