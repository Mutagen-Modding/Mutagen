using Mutagen.Bethesda.Plugins.Exceptions;
using Noggog;

namespace Mutagen.Bethesda.Assets;

public readonly struct DataRelativePath : IEquatable<DataRelativePath>, IComparable<DataRelativePath> 
{
    public static readonly string NullPath = string.Empty;
    public static readonly StringComparison PathComparison = StringComparison.OrdinalIgnoreCase;
    public static readonly StringComparer PathComparer = StringComparer.FromComparison(PathComparison);
    public const string DataDirectory = "Data";
    private static readonly string DataPrefix = DataDirectory + System.IO.Path.DirectorySeparatorChar;
    private static readonly string DataPrefixAlt = DataDirectory + System.IO.Path.AltDirectorySeparatorChar;
    private static readonly string DataInfix = System.IO.Path.DirectorySeparatorChar + DataDirectory + System.IO.Path.DirectorySeparatorChar;
    private static readonly string DataInfixAlt = System.IO.Path.AltDirectorySeparatorChar + DataDirectory + System.IO.Path.AltDirectorySeparatorChar;
    private static readonly int DataPrefixLength = DataDirectory.Length + 1;

    public string Path { get; }

    /// <summary>
    /// Extension of the asset
    /// </summary>
    public string Extension => System.IO.Path.GetExtension(Path);

    public bool IsNull => Path == NullPath;

    public DataRelativePath(string rawPath)
    {
        if (System.IO.Path.IsPathRooted(rawPath))
        {
            AssertHasDataDirectory(rawPath);
        }
        Path = ConvertToDataRelativePath(rawPath);
    }

    internal static bool HasDataDirectory(string filePath)
    {
        var dir = System.IO.Path.GetDirectoryName(filePath);
        while (dir != null)
        {
            var name = System.IO.Path.GetFileName(dir);
            if (name.Equals(DataDirectory, PathComparison))
            {
                return true;
            }

            dir = System.IO.Path.GetDirectoryName(dir);
        }

        return false;
    }

    private static void AssertHasDataDirectory(FilePath filePath)
    {
        if (HasDataDirectory(filePath.Path)) return;

        throw new AssetPathMisalignedException(filePath.Path, "Absolute path did not have Data folder within it.");
    }

    private static string ConvertToDataRelativePath(ReadOnlySpan<char> inputPath)
    {
        Span<char> mySpan = stackalloc char[inputPath.Length];
        inputPath.CopyTo(mySpan);
        IFileSystemExt.CleanDirectorySeparators(mySpan);
        
        ReadOnlySpan<char> path = mySpan;
        
        // Reduce all absolute paths to the path under data directory
        if (path.Contains(System.IO.Path.VolumeSeparatorChar))
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
            .TrimStart(System.IO.Path.DirectorySeparatorChar)
            .TrimStart(System.IO.Path.AltDirectorySeparatorChar);

        int index;
        if ((index = path.IndexOf(DataPrefix, PathComparison)) != -1)
        {
            path = path[(DataPrefixLength + index)..];
        }
        else if ((index = path.IndexOf(DataPrefixAlt, PathComparison)) != -1)
        {
            path = path[(DataPrefixLength + index)..];
        }

        return path.ToString();
    }

    public override string ToString()
    {
        return Path;
    }

    public override int GetHashCode()
    {
        return Path.GetHashCode(PathComparison);
    }

    public override bool Equals(object? obj)
    {
        return obj is DataRelativePath other && Equals(other);
    }

    public bool Equals(DataRelativePath other)
    {
        return PathComparer.Equals(Path, other.Path);
    }

    public int CompareTo(DataRelativePath other)
    {
        return PathComparer.Compare(Path, other.Path);
    }

    public static bool operator ==(DataRelativePath lhs, DataRelativePath rhs)
    {
        return lhs.Equals(rhs);
    }

    public static bool operator !=(DataRelativePath lhs, DataRelativePath rhs)
    {
        return !lhs.Equals(rhs);
    }

    public static implicit operator DataRelativePath(FileInfo info)
    {
        return new FilePath(info.FullName);
    }

    public static implicit operator DataRelativePath(string path)
    {
        return new DataRelativePath(path);
    }

    public static implicit operator DataRelativePath(FilePath path)
    {
        return new DataRelativePath(path);
    }

    public static implicit operator string(DataRelativePath path)
    {
        return path.Path;
    }
}
