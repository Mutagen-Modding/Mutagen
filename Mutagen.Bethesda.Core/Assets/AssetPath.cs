using Noggog;
namespace Mutagen.Bethesda.Assets;

public readonly struct AssetPath : IEquatable<AssetPath>, IComparable<AssetPath> {
    public static readonly string NullPath = string.Empty;
    public static readonly StringComparison PathComparison = StringComparison.OrdinalIgnoreCase;
    public static readonly StringComparer PathComparer = StringComparer.FromComparison(PathComparison);
    public const string DataDirectory = "Data";
    public static readonly string DataPrefix = DataDirectory + Path.DirectorySeparatorChar;
    public static readonly string DataPrefixAlt = DataDirectory + Path.AltDirectorySeparatorChar;
    public static readonly string DataInfix = Path.DirectorySeparatorChar + DataDirectory + Path.DirectorySeparatorChar;
    public static readonly string DataInfixAlt = Path.AltDirectorySeparatorChar + DataDirectory + Path.AltDirectorySeparatorChar;
    public static readonly int DataPrefixLength = DataDirectory.Length + 1;

    public string RawPath { get; }

    public string DataRelativePath => ConvertToDataRelativePath(RawPath);

    public string Extension => Path.GetExtension(RawPath);

    public bool IsNull => RawPath == NullPath;

    public AssetPath(string rawPath)
    {
        RawPath = rawPath;
    }

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

    public override string ToString()
    {
        return DataRelativePath;
    }

    public override int GetHashCode()
    {
        return RawPath.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        return obj is AssetPath other && Equals(other);
    }

    public bool Equals(AssetPath other)
    {
        return PathComparer.Equals(DataRelativePath, other.DataRelativePath);
    }

    public int CompareTo(AssetPath other)
    {
        return PathComparer.Compare(DataRelativePath, other.DataRelativePath);
    }

    public static bool operator ==(AssetPath lhs, AssetPath rhs)
    {
        return lhs.Equals(rhs);
    }

    public static bool operator !=(AssetPath lhs, AssetPath rhs)
    {
        return !lhs.Equals(rhs);
    }

    public static implicit operator AssetPath(FileInfo info)
    {
        return new FilePath(info.FullName);
    }

    public static implicit operator AssetPath(string path)
    {
        return new AssetPath(path);
    }

    public static implicit operator AssetPath(FilePath path)
    {
        return new AssetPath(path);
    }
}
