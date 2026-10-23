using System.IO.Abstractions;

namespace Mutagen.Bethesda;

internal static class FileSystemExt
{
    public static string ResolveIgnoringCase(this IFileSystem fileSystem, string root, string relativePath)
    {
        var exact = Path.Combine(root, relativePath);
        if (fileSystem.File.Exists(exact)) return exact;
        var resolved = root;
        foreach (var name in relativePath.Split(
                     [Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar],
                     StringSplitOptions.RemoveEmptyEntries))
        {
            if (!fileSystem.Directory.Exists(resolved)) return exact;
            var named = Path.Combine(resolved, name);
            if (fileSystem.File.Exists(named) || fileSystem.Directory.Exists(named))
            {
                resolved = named;
                continue;
            }
            var matches = fileSystem.Directory.EnumerateFileSystemEntries(resolved)
                .Where(x => Path.GetFileName(x).Equals(name, StringComparison.OrdinalIgnoreCase))
                .Take(2)
                .ToArray();
            switch (matches.Length)
            {
                case 0:
                    return exact;
                case 1:
                    resolved = matches[0];
                    break;
                default:
                    throw new IOException($"{matches[0]} and {matches[1]} differ only in case, so {exact} could be either.");
            }
        }
        return resolved;
    }
}
