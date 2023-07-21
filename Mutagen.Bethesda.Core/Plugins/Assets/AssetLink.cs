using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins.Exceptions;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Assets;

public static class AssetLink
{
    public static readonly string NullPath = string.Empty;
    public static readonly StringComparison PathComparison = StringComparison.OrdinalIgnoreCase;
    public static readonly StringComparer PathComparer = StringComparer.FromComparison(PathComparison);
    public const string DataDirectory = "Data";
    public static readonly string DataPrefix = DataDirectory + Path.DirectorySeparatorChar;
    public static readonly string DataPrefixAlt = DataDirectory + Path.AltDirectorySeparatorChar;
    public static readonly string DataInfix = Path.DirectorySeparatorChar + DataDirectory + Path.DirectorySeparatorChar;
    public static readonly string DataInfixAlt = Path.AltDirectorySeparatorChar + DataDirectory + Path.AltDirectorySeparatorChar;
    public static readonly int DataPrefixLength = DataDirectory.Length + 1;

    public static string ConvertToDataRelativePath(ReadOnlySpan<char> inputPath)
    {
        Span<char> mySpan = stackalloc char[inputPath.Length];
        inputPath.CopyTo(mySpan);
        IFileSystemExt.CleanDirectorySeparators(mySpan);
        
        ReadOnlySpan<char> path = mySpan;
        
        // Reduce all absolute paths to the path under data directory
        if (path.Contains(Path.VolumeSeparatorChar))
        {
            var dataDirectoryIndex = path.IndexOf(DataInfix, PathComparison);
            if (dataDirectoryIndex != -1)
            {
                path = path[(dataDirectoryIndex + DataInfix.Length)..];
            }
            else
            {
                dataDirectoryIndex = path.IndexOf(DataInfixAlt, PathComparison);
                if (dataDirectoryIndex != -1)
                {
                    path = path[(dataDirectoryIndex + DataInfixAlt.Length)..];
                }
            }
        }

        path = path
            .TrimStart(Path.DirectorySeparatorChar)
            .TrimStart(Path.AltDirectorySeparatorChar);

        // Can be replaced with a version of TrimStart that takes the string comparison into account
        if (path.StartsWith(DataPrefix, PathComparison))
        {
            path = path[DataPrefixLength..];
        }
        else if (path.StartsWith(DataPrefixAlt, PathComparison))
        {
            path = path[DataPrefixLength..];
        }

        return path.ToString();
    }
}

/// <summary>
/// Asset referenced by a record
/// </summary>
public class AssetLinkGetter<TAssetType> : IComparable<AssetLinkGetter<TAssetType>>, IAssetLinkGetter<TAssetType>
    where TAssetType : class, IAssetType
{
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
        _rawPath = AssetLink.NullPath;
    }

    public AssetLinkGetter(string rawPath)
    {
        _rawPath = rawPath;
    }

    public string RawPath => _rawPath;

    public string DataRelativePath => ConvertToDataRelativePath(RawPath);

    public string Extension => Path.GetExtension(RawPath);

    public IAssetType Type => AssetInstance;

    public static string ConvertToDataRelativePath(ReadOnlySpan<char> inputPath)
    {
        Span<char> mySpan = stackalloc char[inputPath.Length];
        inputPath.CopyTo(mySpan);
        IFileSystemExt.CleanDirectorySeparators(mySpan);
        
        ReadOnlySpan<char> path = mySpan;
        
        // Reduce all absolute paths to the path under data directory
        if (path.Contains(Path.VolumeSeparatorChar))
        {
            var dataDirectoryIndex = path.IndexOf(AssetLink.DataInfix, AssetLink.PathComparison);
            if (dataDirectoryIndex != -1)
            {
                path = path[(dataDirectoryIndex + AssetLink.DataInfix.Length)..];
            }
            else
            {
                dataDirectoryIndex = path.IndexOf(AssetLink.DataInfixAlt, AssetLink.PathComparison);
                if (dataDirectoryIndex != -1)
                {
                    path = path[(dataDirectoryIndex + AssetLink.DataInfixAlt.Length)..];
                }
            }
        }

        path = path
            .TrimStart(Path.DirectorySeparatorChar)
            .TrimStart(Path.AltDirectorySeparatorChar);

        // Can be replaced with a version of TrimStart that takes the string comparison into account
        if (path.StartsWith(AssetLink.DataPrefix, AssetLink.PathComparison))
        {
            path = path[AssetLink.DataPrefixLength..];
        }
        else if (path.StartsWith(AssetLink.DataPrefixAlt, AssetLink.PathComparison))
        {
            path = path[AssetLink.DataPrefixLength..];
        }

        return path.StartsWith(AssetInstance.BaseFolder, AssetLink.PathComparison) 
            ? path.ToString() 
            : Path.Combine(AssetInstance.BaseFolder, path.ToString());
    }

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
        return AssetLink.PathComparer.GetHashCode(DataRelativePath);
    }

    public int CompareTo(AssetLinkGetter<TAssetType>? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;

        return AssetLink.PathComparer.Compare(DataRelativePath, other.DataRelativePath);
    }
    
    public bool IsNull => RawPath == AssetLink.NullPath;

    [return: NotNullIfNotNull("asset")]
    public static implicit operator string? (AssetLinkGetter<TAssetType>? asset) => asset?.RawPath;

    [return: NotNullIfNotNull("path")]
    public static implicit operator AssetLinkGetter<TAssetType>?(string? path) => path == null ? null : new(path);

    [return: NotNullIfNotNull("path")]
    public static implicit operator AssetLinkGetter<TAssetType>?(FilePath? path) => path == null ? null : new(path.Value.Path);
}

/// <summary>
/// Asset referenced by a record
/// </summary>
public class AssetLink<TAssetType> : 
    AssetLinkGetter<TAssetType>, 
    IComparable<AssetLink<TAssetType>>,
    IAssetLink<AssetLink<TAssetType>, TAssetType>,
    IAssetLink<TAssetType>
    where TAssetType : class, IAssetType
{
    public AssetLink()
        : base(AssetLink.NullPath)
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
        
        if (path.StartsWith(AssetInstance.BaseFolder, AssetLink.PathComparison))
        {
            _rawPath = path[AssetInstance.BaseFolder.Length..];
            _rawPath = _rawPath
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
        get => _rawPath;
        set => _rawPath = value;
    }

    public AssetLink<TAssetType> ShallowClone()
    {
        return new AssetLink<TAssetType>(RawPath);
    }

    public void SetToNull()
    {
        _rawPath = AssetLink.NullPath;
    }

    public override string ToString()
    {
        return DataRelativePath;
    }

    public override int GetHashCode()
    {
        return AssetLink.PathComparer.GetHashCode(RawPath);
    }

    public override bool Equals(object? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        if (other is not AssetLink<TAssetType> rhs) return false;

        return AssetLink.PathComparer.Equals(RawPath, rhs.RawPath);
    }

    public virtual bool Equals(AssetLink<TAssetType>? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return AssetLink.PathComparer.Equals(RawPath, other.RawPath);
    }

    public int CompareTo(AssetLink<TAssetType>? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;

        return AssetLink.PathComparer.Compare(RawPath, other.RawPath);
    }

    [return: NotNullIfNotNull("asset")]
    public static implicit operator string?(AssetLink<TAssetType>? asset) => asset?.RawPath!;

    [return: NotNullIfNotNull("path")]
    public static implicit operator AssetLink<TAssetType>?(string? path) => path == null ? null : new(path);

    [return: NotNullIfNotNull("path")]
    public static implicit operator AssetLink<TAssetType>?(FilePath? path) => path == null ? null : new(path.Value.Path);
}
