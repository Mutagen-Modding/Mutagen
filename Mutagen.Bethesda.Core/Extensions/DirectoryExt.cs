using System.IO.Abstractions;

namespace Mutagen.Bethesda;

internal static class DirectoryExt
{
    // The game and Windows match file names ignoring case. Every other option is the
    // default of the EnumerateFiles overload that takes none.
    private static readonly EnumerationOptions IgnoringCase = new()
    {
        MatchCasing = MatchCasing.CaseInsensitive,
        MatchType = MatchType.Win32,
        AttributesToSkip = 0,
        IgnoreInaccessible = false,
    };

    public static IEnumerable<string> EnumerateFilesIgnoringCase(this IDirectory directory, string path, string searchPattern)
    {
        return directory.EnumerateFiles(path, searchPattern, IgnoringCase);
    }
}
