using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins.Exceptions;

namespace Mutagen.Bethesda.Plugins.Assets;

/// <summary>
/// Asset referenced by a record
/// </summary>
public class AssetLinkGetter<TAssetType> : IComparable<AssetLinkGetter<TAssetType>>, IAssetLinkGetter<TAssetType>
    where TAssetType : IAssetType
{
    protected const string DataDirectory = "Data";
    protected static readonly string DataPrefix = DataDirectory + Path.DirectorySeparatorChar;
    protected static readonly string DataPrefixAlt = DataDirectory + Path.AltDirectorySeparatorChar;
    protected static readonly string DataInfix = Path.DirectorySeparatorChar + DataDirectory + Path.DirectorySeparatorChar;
    protected static readonly string DataInfixAlt = Path.AltDirectorySeparatorChar + DataDirectory + Path.AltDirectorySeparatorChar;
    protected static readonly int DataPrefixLength = DataDirectory.Length + 1;

    protected string _rawPath;
    protected static readonly TAssetType AssetInstance;
    public static readonly AssetLinkGetter<TAssetType> Null = new();

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

    public string DataRelativePath
    {
        get {
            var path = RawPath;
            
            // Reduce all absolute paths to the path under data directory
            if (path.Contains(Path.VolumeSeparatorChar)) {
                var dataDirectoryIndex = path.IndexOf(DataInfix, IAssetLinkGetter.PathComparison);
                if (dataDirectoryIndex != -1) {
                    path = path[(dataDirectoryIndex + DataInfix.Length)..];
                } else {
                    dataDirectoryIndex = path.IndexOf(DataInfixAlt, IAssetLinkGetter.PathComparison);
                    if (dataDirectoryIndex != -1) {
                        path = path[(dataDirectoryIndex + DataInfixAlt.Length)..];
                    }
                }
            }
            
            path = path
                .TrimStart(Path.DirectorySeparatorChar)
                .TrimStart(Path.AltDirectorySeparatorChar);
            
            // Can be replaced with a version of TrimStart that takes the string comparison into account
            if (path.StartsWith(DataPrefix, IAssetLinkGetter.PathComparison)) path = path[DataPrefixLength..];
            else if (path.StartsWith(DataPrefixAlt, IAssetLinkGetter.PathComparison)) path = path[DataPrefixLength..];

            return path.StartsWith(AssetInstance.BaseFolder, IAssetLinkGetter.PathComparison) ? path : Path.Combine(AssetInstance.BaseFolder, path);
        }
    }

    public string Extension => Path.GetExtension(RawPath).TrimStart('.');

    public IAssetType Type => AssetInstance;

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
        return IAssetLinkGetter.PathComparer.GetHashCode(DataRelativePath);
    }

    public int CompareTo(AssetLinkGetter<TAssetType>? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;

        return IAssetLinkGetter.PathComparer.Compare(DataRelativePath, other.DataRelativePath);
    }
    
    public bool IsNull => RawPath == IAssetLinkGetter.NullPath;

    [return: NotNullIfNotNull("asset")]
    public static implicit operator string? (AssetLinkGetter<TAssetType>? asset) => asset?.RawPath;

    [return: NotNullIfNotNull("path")]
    public static implicit operator AssetLinkGetter<TAssetType>?(string? path) => path == null ? null : new(path);
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

    [return: NotNullIfNotNull("asset")]
    public static implicit operator string?(AssetLink<TAssetType>? asset) => asset?.RawPath!;

    [return: NotNullIfNotNull("path")]
    public static implicit operator AssetLink<TAssetType>?(string? path) => path == null ? null : new(path);
}
