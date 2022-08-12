using System.Runtime.CompilerServices;
using Noggog.IO;

namespace Mutagen.Bethesda.UnitTests;

public static class TestPathing
{
    public static readonly string TempFolderPath = "MutagenUnitTests";

    public static TempFolder GetTempFolder(string folderName, [CallerMemberName] string? testName = null)
    {
        return TempFolder.FactoryByAddedPath(Path.Combine(TempFolderPath, folderName, testName!));
    }
}