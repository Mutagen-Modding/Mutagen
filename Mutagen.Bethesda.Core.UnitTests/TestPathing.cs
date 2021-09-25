using System.IO;
using System.Runtime.CompilerServices;
using Mutagen.Bethesda.Plugins;
using Noggog.Utility;

namespace Mutagen.Bethesda.Core.UnitTests
{
    public static class TestPathing
    {
        public static readonly string TempFolderPath = "MutagenUnitTests";

        public static TempFolder GetTempFolder(string folderName, [CallerMemberName] string? testName = null)
        {
            return TempFolder.FactoryByAddedPath(Path.Combine(TempFolderPath, folderName, testName!));
        }
    }
}
